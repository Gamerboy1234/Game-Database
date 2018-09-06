using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class Rating
    {
        #region Constants

        public const string TableName = "Rating";

        public const int NameSize = 100;
        public const int DescriptionSize = 100;
        public const int SymbolSize = 10;

        #endregion Constants 

        #region Properties 

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }

        #endregion Properties 

        #region Constructers 

        public Rating()
        {
            Id = 0;
            Name = "";
            Description = "";
            Symbol = "";
        }

        public Rating(int id, string name, string maker, string symbol)
        {
            Id = id;
            Name = name;
            Description = maker;
            Symbol = symbol;
        }
        #endregion Constructers

        #region Public Methods

        public string GenerateInsertStatment()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (Name, Description, Symbol) VALUES ('{Name}', '{Description}', '{Symbol}')";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public string GenerateUpdateStatement()
        {
            var result = "";
            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                result = $"UPDATE {TableName} SET Name = '{Name}', Description = '{Description}', Symbol = '{Symbol}' WHERE {whereClause}";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public string GenerateDeleteStatement()
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"DELETE FROM {TableName} WHERE {whereClause}";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public string GenerateExistQuery()
        {
            var result = "";

            try
            {
                var whereclause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereclause))
                {
                    result = $"Select Id FROM {TableName} WHERE {whereclause}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public string GenerateSelectQuery()
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                if (string.IsNullOrEmpty(whereClause) &&
                    !string.IsNullOrEmpty(Name))
                {
                    whereClause = $"Name = '{Name}'";
                }

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"SELECT Id, Name, Description FROM {TableName} WHERE {whereClause}";
                }

                else
                {
                    result = $"SELECT Id, Name, Description FROM {TableName}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public string GeneratePrimaryKeyWhereClause()
        {
            var result = "";

            try
            {
                if (Id > 0)
                {
                    result = $"Id = {Id}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Public Methods

        #region Dictionary Methods

        public static Dictionary<string, object> ToDictionary(Rating rating)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (rating != null)
                {
                    result.Add("Id", rating.Id);
                    result.Add("Name", rating.Name ?? "");
                    result.Add("Description", rating.Description ?? "");
                    result.Add("Symbol", rating.Symbol ?? "");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static Rating FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new Rating();

            try
            {
                if (dictionary?.Count > 0)
                {
                    foreach (var key in dictionary.Keys)
                    {
                        switch (key.Trim())
                        {
                            case "Id":
                                result.Id = Convert.ToInt32(dictionary[key]);
                                break;
                            case "Name":
                                result.Name = dictionary[key] as string;
                                break;
                            case "Description":
                                result.Description = dictionary[key] as string;
                                break;
                              case "Symbol":
                                result.Symbol = dictionary[key] as string;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Dictionary Methods
    }
    [Serializable]
    public class RatingList
    {
        #region properties

        public List<Rating> List { get; } = new List<Rating>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public Rating GetbyId(int id)
        {
            try
            {
                foreach (var rating in List.Where(rating => (rating != null) && (rating.Id == id)))
                {
                    return rating;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;

        }
        public Rating GetByName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var rating in List.Where(rating => (rating != null) && string.Equals(rating.Name, name)))
                    {
                        return rating;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }

        public bool Exists(int id)
        {
            var result = false;
            try
            {
                result = (GetbyId(id) != null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool Add(Rating rating)
        {
            var result = false;

            try
            {
                if (rating != null)
                {
                    if (!Exists(rating.Id))
                    {
                        List.Add(rating);

                        result = Exists(rating.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public void Remove(int id)
        {
            try
            {
                var RemoveRating = GetbyId(id);

                if (RemoveRating != null)
                {
                    List.Remove(RemoveRating);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        #endregion Public Methods

        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(RatingList ratinglist)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                result = (from rating in ratinglist.List where rating != null select Rating.ToDictionary(rating) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static RatingList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new RatingList();

            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary.Count > 0))
                    {
                        result.List.Add(Rating.FromDictionary(dataDictionary));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Dictionary Methods 

    }
}
