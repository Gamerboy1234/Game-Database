
using System;
using Logger;

namespace GameLibrary.Core
{
    public static class GameLibraryAgent
    {
        #region Properties

        public static ModelAssembler ModelAssembler { get; private set; }

        #endregion Properties


        #region Public Members

        public static void Startup(string connectionString)
        {
            try
            {
                ModelAssembler = new ModelAssembler(connectionString);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void Shutdown()
        {
            try
            {
                ModelAssembler = null;
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Public Members
    }
}
