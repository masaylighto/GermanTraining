
using Logic.Core.DataType;
using OneOf;

namespace Logic.Core.Models;

/// <summary>
/// to make a value read only and not depend on the need
/// </summary>
public class Freezable<DataType>
{
    /// <summary>
    ///  create a Freezable object (frozen by default)
    /// </summary>
    public Freezable(DataType data)
    {
        _data = data;
        _isFrozen = true;
    }
    DataType _data;
    bool _isFrozen;
    /// <summary>
    /// make value read only 
    /// </summary>
    public void Freeze() 
    {
        _isFrozen=true;
    }
    /// <summary>
    /// make value editable 
    /// </summary>
    public void UnFreeze() 
    {
        _isFrozen = false;
    }
    public DataType Value => _data;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns>if value is frozen and you attempt to add data to it it will return exception as value</returns>
    public OneOf<Ok,Exception> SetValue(DataType value)
    {
        if (!_isFrozen)
        {
            _data = value;
            return new Ok();
        }
        return new Exception("value is frozen");
    }
    /// <summary>
    ///  create a Freezable object (frozen by default)
    /// </summary>
    public static implicit operator Freezable<DataType>(DataType data) => new Freezable<DataType>(data);
   
    public static implicit operator DataType(Freezable<DataType> data) => data.Value;

}
