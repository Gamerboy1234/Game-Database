using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class Review
    {
        #region Constants

        public const string TableName = "Review";

        public const int NameSize = 100;
        public const int DescriptionSize = 1000;

        #endregion Constants 

        #region Properties 

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Reviewrating { get; set; }

        #endregion Properties 

        #region Constructers 

        public Review()
        {
            Id = 0;
            Name = "";
            Description = "";
            Reviewrating = 0;
        }

        public Review(int id, string name, string description, int review)
        {
            Id = id;
            Name = name;
            Description = description;
            Reviewrating = review;
        }

        #endregion Constructers

        #region Public Methods

        public string GenerateInsertStatment()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (Name, Description, Rating) VALUES ('{Name}', '{Description}', {Reviewrating})";
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

                result = $"UPDATE {TableName} SET Name = '{Name}', Description = '{Description}', Rating = {Reviewrating} WHERE = {whereClause}";
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
                var whereclause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereclause))
                {
                    result = $"SELECT Id, Name, Description, Rating FROM {TableName} WHERE {whereclause}";
                }
                else
                {
                    result = $"SELECT Id, Name, Description, Rating FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(Review review)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (review != null)
                {
                    result.Add("Id", review.Id);
                    result.Add("Name", review.Name ?? "");
                    result.Add("Description", review.Description ?? "");
                    result.Add("Review", review.Reviewrating);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static Review FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new Review();

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
                            case "Review":
                                result.Reviewrating = Convert.ToInt32(dictionary[key]);
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
    public class ReviewList
    {
        #region properties

        public List<Review> List { get; } = new List<Review>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public Review GetbyId(int id)
        {
            try
            {
                foreach (var review in List.Where(rating => (rating != null) && (rating.Id == id)))
                {
                    return review;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;

        }
        public Review GetByName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var review in List.Where(rating => (rating != null) && string.Equals(rating.Name, name)))
                    {
                        return review;
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

        public bool Add(Review rating)
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
                var RemoveReview = GetbyId(id);

                if (RemoveReview != null)
                {
                    List.Remove(RemoveReview);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        #endregion Public Methods

        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(ReviewList ratinglist)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                result = (from rating in ratinglist.List where rating != null select Review.ToDictionary(rating) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static ReviewList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new ReviewList();

            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary.Count > 0))
                    {
                        result.List.Add(Review.FromDictionary(dataDictionary));
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
