using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace GameLibrary.Model
{
    [Serializable]
    public class Platform
    {
        #region Constants

        public const string TableName = "Platform";

        public const int NameSize = 100;
        public const int MakerSize = 100;

        #endregion Constants 

        #region Properties 

        public int Id { get; set; }
        public string Name { get; set; }
        public string Maker { get; set; }

        #endregion Properties 

        #region Constructers 

        public Platform()
        {
            Id = 0;
            Name = "";
            Maker = "";
        }

        public Platform(int id, string name, string maker)
        {
            Id = id;
            Name = name;
            Maker = maker;
        }
        #endregion Constructers

        #region Public Methods

        public string GenerateInsertStatment()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (Name, Maker) VALUES ('{Name}', '{Maker}')";
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

                result = $"UPDATE {TableName} SET Name = '{Name}', Maker = '{Maker}' WHERE {whereClause}";
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
                    result = $"SELECT Id, Name, Maker FROM {TableName} WHERE {whereClause}";
                }

                else
                {
                    result = $"SELECT Id, Name, Maker FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(Platform platfrom)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (platfrom != null)
                {
                    result.Add("Id", platfrom.Id);
                    result.Add("Name", platfrom.Name ?? "");
                    result.Add("Name", platfrom.Maker ?? "");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static Platform FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new Platform();

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
                            case "Maker":
                                result.Maker = dictionary[key] as string;
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
    public class PlatformList
    {
        #region properties

        public List<Platform> List { get; } = new List<Platform>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties
        
        #region Public Methods

        public Platform GetbyId(int id)
        {
            try
            {
                foreach (var platform in List.Where(platform => (platform != null) && (platform.Id == id)))
                {
                    return platform;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;

        }
        public Platform GetByName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var platform in List.Where(platform => (platform != null) && string.Equals(platform.Name, name)))
                    {
                        return platform;
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

        public bool Add(Platform platform)
        {
            var result = false;

            try
            {
                if (platform != null)
                {
                    if (!Exists(platform.Id))
                    {
                        List.Add(platform);

                        result = Exists(platform.Id);
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
                var RemovePlatform = GetbyId(id);

                if (RemovePlatform != null)
                {
                    List.Remove(RemovePlatform);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        #endregion Public Methods

        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(PlatformList platformlist)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                result = (from platform in platformlist.List where platform != null select Platform.ToDictionary(platform) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static PlatformList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new PlatformList();

            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary.Count > 0))
                    {
                        result.List.Add(Platform.FromDictionary(dataDictionary));
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
