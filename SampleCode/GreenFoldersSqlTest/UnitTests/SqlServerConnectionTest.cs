
using System;
using System.Text;
using GreenFolders.Sql;
using Xunit;

namespace GreenFoldersSqlTest.UnitTests
{
    public class SqlServerConnectionTest
    {
        #region Private Members

        private const string ConnectionString = "Server=.;Database=GreenFolders;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [Fact]
        public void SqlServerConnectionConstructorAndConnectionTest()
        {
            var connection = new SqlServerConnection();

            Assert.Equal(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.Equal(result, true);

            Assert.Equal(connection.IsConnected, true);

            connection = new SqlServerConnection(ConnectionString, 0);

            Assert.Equal(connection.IsConnected, true);
        }

        [Fact]
        public void SqlServerConnectionExecuteCommandAndExecuteQueryTest()
        {
            var connection = new SqlServerConnection(ConnectionString, 0);

            Assert.Equal(connection.IsConnected, true);

            var attachmentTypeGuid = Guid.NewGuid();

            var errorMessage = "";
            var result = connection.ExecuteCommand($"INSERT INTO AttachmentTypes ([AttachmentTypeGuid], [ParentGuid], [Name], [Description], [StoreCompressed], [StoreEncrypted], [OrderNumber], [AssociatedObjects], [ReplaceAssociatedObjects]) VALUES ('{attachmentTypeGuid}', '{Guid.NewGuid()}', 'Test Attachment Type', 'Test Description', 0, 0, 1, NULL, 0)", ref errorMessage);

            Assert.Equal(result, true);

            const string text = "abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ";

            result = connection.WriteBlobData($"UPDATE AttachmentTypes SET Description=@Description WHERE AttachmentTypeGuid = '{attachmentTypeGuid}'", "Description", "varchar", text, null);

            Assert.Equal(result, true);

            var dataResult = connection.ReadBlobData($"SELECT Description FROM AttachmentTypes WHERE AttachmentTypeGuid = '{attachmentTypeGuid}'", 0);

            Assert.NotNull(dataResult);

            var textResult = Encoding.UTF8.GetString(dataResult);

            Assert.Equal(textResult, text);

            result = connection.ExecuteCommand($"DELETE AttachmentTypes WHERE AttachmentTypeGuid = '{attachmentTypeGuid}'", ref errorMessage);

            Assert.Equal(result, true);
        }

        #endregion Test Cases
    }
}
