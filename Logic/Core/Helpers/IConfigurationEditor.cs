
using Logic.Core.DataType;
using OneOf;

namespace Logic.Core.Helpers;

public interface IConfigurationEditor
{
    OneOf<Ok, Exception> Parse(string settingsFilePath);
    OneOf<Ok, Exception> SetValue<DataType>(string key, DataType value) where DataType : class;
}
