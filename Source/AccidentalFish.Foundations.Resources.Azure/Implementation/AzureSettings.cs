namespace AccidentalFish.Foundations.Resources.Azure.Implementation
{
    class AzureSettings : IAzureSettings
    {
        public AzureSettings(bool createIfNotExists)
        {
            CreateIfNotExists = createIfNotExists;
        }

        public bool CreateIfNotExists { get; }
    }
}
