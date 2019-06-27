using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedAggregate(
    Format.UserDefined, //use clr serialization to serialize the intermediate result
    IsInvariantToNulls = true, //optimizer property
    IsInvariantToDuplicates = false, //optimizer property
    IsInvariantToOrder = false, //optimizer property
    MaxByteSize = 8000) //maximum size in bytes of persisted value
]
public class Product : IBinarySerialize
{
    /// <summary>
    /// The variable that holds the intermediate result of the concatenation
    /// </summary>
    public SqlDecimal intermediateResult;

    /// <summary>
    /// Initialize the internal data structures  
    /// </summary>
    public void Init()
    {
    }

    /// <summary>
    /// Accumulate the next value, not if the value is null
    /// </summary>
    /// <param name="value"></param>
    public void Accumulate(SqlDecimal value)
    {
        if (!value.IsNull)
        {
            this.intermediateResult = this.intermediateResult.IsNull ? value.Value : this.intermediateResult.Value * value.Value;
        }
    }

    /// <summary>
    /// Merge the partially computed aggregate with this aggregate.
    /// </summary>
    /// <param name="other"></param>
    public void Merge(Product other)
    {
        if (!other.intermediateResult.IsNull)
        {
            this.intermediateResult = this.intermediateResult.IsNull ? other.intermediateResult.Value : this.intermediateResult.Value * other.intermediateResult.Value;
        }
    }

    /// <summary>
    /// Called at the end of aggregation, to return the results of the aggregation.
    /// </summary>
    /// <returns></returns>
    public SqlDecimal Terminate()
    {
        return this.intermediateResult;
    }

    public void Read(BinaryReader r)
    {
        intermediateResult = r.ReadDecimal();
    }

    public void Write(BinaryWriter w)
    {
        if (!this.intermediateResult.IsNull)
        {
            w.Write(this.intermediateResult.Value);
        }
    }
}