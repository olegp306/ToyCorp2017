//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class OfferService
    {
        public static int AddOffer(Offer offer)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddOffer]", CommandType.StoredProcedure,
                                          new SqlParameter("@ProductID", offer.ProductId),
                                          new SqlParameter("@ArtNo", offer.ArtNo),
                                          new SqlParameter("@Amount", offer.Amount),
                                          new SqlParameter("@Price", offer.Price),
                                          new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                          new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                          new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                          new SqlParameter("@Main", offer.Main));
        }

        public static void UpdateOffer(Offer offer)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOffer]", CommandType.StoredProcedure,
                                            new SqlParameter("@OfferId", offer.OfferId),
                                            new SqlParameter("@ProductID", offer.ProductId),
                                            new SqlParameter("@ArtNo", offer.ArtNo),
                                            new SqlParameter("@Amount", offer.Amount),
                                            new SqlParameter("@Price", offer.Price),
                                            new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                            new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                            new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                            new SqlParameter("@Main", offer.Main));
        }

        public static void DeleteOffer(int offerID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Catalog].[Offer] WHERE [offerID] = @offerID",
                                          CommandType.Text,
                                          new SqlParameter("@offerID", offerID));
        }


        public static void DeleteOldOffers(int productId, List<Offer> newOffers)
        {
            string query = newOffers != null && newOffers.Any()
                               ? string.Format("Delete FROM [Catalog].[Offer] WHERE [productId] = @productId and offerID not in ({0})",
                                    newOffers.Select(offer => offer.OfferId).AggregateString(','))
                               : "Delete FROM [Catalog].[Offer] WHERE [productId] = @productId";

            SQLDataAccess.ExecuteNonQuery(query, CommandType.Text,
                new SqlParameter("@productId", productId));
        }

        public static List<Offer> GetProductOffers(int productID)
        {
            return SQLDataAccess.ExecuteReadList<Offer>(
                     "SELECT * FROM [Catalog].[Offer] WHERE [ProductID] = @ProductID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ProductID", productID));
        }

        public static Offer GetOffer(int offerID)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT * FROM [Catalog].[Offer] WHERE [offerID] = @offerID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@offerID", offerID));
        }

        public static Offer GetOffer(string artNo)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT * FROM [Catalog].[Offer] WHERE [artNo] = @ArtNo",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ArtNo", artNo));
        }

        public static Offer GetMainOffer(List<Offer> offers, bool allowPreOrder, int? colorid = null, int? sizeId = null)
        {
            Offer CurrentOffer = null;

            if (colorid.HasValue && sizeId.HasValue)
            {
                CurrentOffer = offers.FirstOrDefault(o => o.ColorID == colorid && o.SizeID == sizeId);
            }
            else if (colorid.HasValue)
            {
                CurrentOffer = offers.FirstOrDefault(o => o.ColorID == colorid);
            }
            else if (sizeId.HasValue)
            {
                CurrentOffer = offers.FirstOrDefault(o => o.SizeID == sizeId);
            }

            if (CurrentOffer == null)
            {
                CurrentOffer = offers.OrderByDescending(o => o.Main).ThenBy(o => o.Color != null ? o.Color.SortOrder : 0)
                                                                    .ThenBy(o => o.Size != null ? o.Size.SortOrder : 0)
                                                                    .FirstOrDefault(o => (o.Price > 0 && o.Amount > 0) || allowPreOrder);
            }

            return CurrentOffer ?? offers.FirstOrDefault();
        }

        public static Offer GetOfferFromReader(SqlDataReader reader)
        {
            return new Offer
            {
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                Price = SQLDataHelper.GetFloat(reader, "Price"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                ColorID = SQLDataHelper.GetNullableInt(reader, "ColorID"),
                SizeID = SQLDataHelper.GetNullableInt(reader, "SizeID"),
                Main = SQLDataHelper.GetBoolean(reader, "Main")
            };
        }

        public static bool IsArtNoExist(string artNo, int offerId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select Count(OfferID) from Catalog.Offer Where ArtNo=@ArtNo and OfferID<>@OfferID",
                    CommandType.Text, new SqlParameter("@ArtNo", artNo),
                    new SqlParameter("@offerID", offerId)
                    ) > 0;
        }

        public static string OffersToString(List<Offer> offers, string columSeparator, string propertySeparator)
        {
            return offers.OrderByDescending(o => o.Main).Select(offer =>
                                 offer.ArtNo + propertySeparator + (offer.Size != null ? offer.Size.SizeName : "null") + propertySeparator +
                                 (offer.Color != null ? offer.Color.ColorName : "null") + propertySeparator + offer.Price +
                                 propertySeparator + offer.SupplyPrice + propertySeparator + offer.Amount).AggregateString(columSeparator);
        }

        public static void OffersFromString(Product product, string offersString, string columSeparator,string propertySeparator)
        {
            if (string.IsNullOrEmpty(columSeparator) || string.IsNullOrEmpty(propertySeparator))
                _OffersFromString(product, offersString);
            else
                _OffersFromString(product, offersString, columSeparator, propertySeparator);
        }

        private static void _OffersFromString(Product product, string offersString)
        {
            product.HasMultiOffer = true;

            var oldOffers = new List<Offer>(product.Offers);
            product.Offers.Clear();

            var mainOffer = true;

            foreach (string[] fields in offersString.Split(';').Select(str => str.Replace("[", "").Replace("]", "").Split(':')))
            {
                if (fields.Count() == 6)
                {
                    var multiOffer = oldOffers.FirstOrDefault(offer => offer.ArtNo == fields[0]) ?? new Offer();
                    multiOffer.ProductId = product.ProductId;
                    multiOffer.Main = mainOffer;

                    multiOffer.ArtNo = fields[0]; // ArtNo

                    if (fields[1] != "null") // Size
                    {
                        Size size = SizeService.GetSize(fields[1]);
                        if (size == null)
                        {
                            size = new Size { SizeName = fields[1] };
                            size.SizeId = SizeService.AddSize(size);
                        }

                        multiOffer.SizeID = size.SizeId;
                    }
                    else
                    {
                        multiOffer.SizeID = null;
                    }

                    if (fields[2] != "null") // Color
                    {
                        Color color = ColorService.GetColor(fields[2]);
                        if (color == null)
                        {
                            color = new Color { ColorName = fields[2], ColorCode = "#000000" };
                            color.ColorId = ColorService.AddColor(color);
                        }

                        multiOffer.ColorID = color.ColorId;
                    }
                    else
                    {
                        multiOffer.ColorID = null;
                    }


                    multiOffer.Price = fields[3].TryParseFloat(); // Price
                    multiOffer.SupplyPrice = fields[4].TryParseFloat(); // SupplyPrice
                    multiOffer.Amount = fields[5].TryParseFloat(); //Amount

                    product.Offers.Add(multiOffer);
                    mainOffer = false;
                }
            }
        }

        private static void _OffersFromString(Product product, string offersString, string columSeparator, string propertySeparator)
        {
            product.HasMultiOffer = true;

            var oldOffers = new List<Offer>(product.Offers);
            product.Offers.Clear();

            var mainOffer = true;

            foreach (string[] fields in offersString.Replace("[", "").Replace("]", "").Split(columSeparator).Select(str => str.Split(propertySeparator)))
            {
                if (fields.Count() == 6)
                {
                    var multiOffer = oldOffers.FirstOrDefault(offer => offer.ArtNo == fields[0]) ?? new Offer();
                    multiOffer.ProductId = product.ProductId;
                    multiOffer.Main = mainOffer;

                    multiOffer.ArtNo = fields[0]; // ArtNo

                    if (fields[1] != "null") // Size
                    {
                        Size size = SizeService.GetSize(fields[1]);
                        if (size == null)
                        {
                            size = new Size { SizeName = fields[1] };
                            size.SizeId = SizeService.AddSize(size);
                        }

                        multiOffer.SizeID = size.SizeId;
                    }
                    else
                    {
                        multiOffer.SizeID = null;
                    }

                    if (fields[2] != "null") // Color
                    {
                        Color color = ColorService.GetColor(fields[2]);
                        if (color == null)
                        {
                            color = new Color { ColorName = fields[2], ColorCode = "#000000" };
                            color.ColorId = ColorService.AddColor(color);
                        }

                        multiOffer.ColorID = color.ColorId;
                    }
                    else
                    {
                        multiOffer.ColorID = null;
                    }

                    multiOffer.Price = fields[3].TryParseFloat(); // Price
                    multiOffer.SupplyPrice = fields[4].TryParseFloat(); // SupplyPrice
                    multiOffer.Amount = fields[5].TryParseFloat(); //Amount

                    product.Offers.Add(multiOffer);
                    mainOffer = false;
                }
            }
        }

        public static void OfferFromFields(Product product, float? price, float? purchase, float? amount)
        {
            
            if (price == null && purchase == null && amount == null)
                return;

            product.HasMultiOffer = false;

            var singleOffer = product.Offers.FirstOrDefault() ?? new Offer();
            product.Offers.Clear();

            singleOffer.ArtNo = product.ArtNo;
            singleOffer.Main = true;
            singleOffer.ProductId = product.ProductId;
            singleOffer.Price = price ?? singleOffer.Price;
            singleOffer.SupplyPrice = purchase ?? singleOffer.SupplyPrice;
            singleOffer.Amount = amount ?? singleOffer.Amount;
            singleOffer.ColorID = null;
            singleOffer.SizeID = null;

            product.Offers.Add(singleOffer);
        }
    }
}