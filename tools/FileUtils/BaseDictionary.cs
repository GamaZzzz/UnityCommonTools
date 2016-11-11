using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class BaseDictionary<TKey, TValue>
{
    public bool Loaded { get; private set; }
    public bool Success { get; private set; }
    protected Dictionary<TKey, TValue> _dictrionary;
    protected abstract string Path { get; }
    protected abstract string BasePath { get; }

    public void LoadAsync()
    {
        FileReader reader = FileReader.CreateReader(BasePath, Path, FileReader.FileType.CSVFILE);
        reader.OnReadCompleted = (ret) => {
            Loaded = true;
            if (ret.Success)
            {
                ParseLine(((CSVFileReader)ret).Lines);
            }
            Success = ret.Success;
        };
        reader.ReadAsync();
    }

    protected abstract void ParseLine(List<string[]> datas);

    public bool ContainsKey(TKey key)
    {
        return _dictrionary.ContainsKey(key);
    }

    public TValue Get(TKey key)
    {
        if (_dictrionary.ContainsKey(key))
        {
            return _dictrionary[key];
        }
        return default(TValue);
    }

    public List<TValue> GetAllValues()
    {
        return new List<TValue>(_dictrionary.Values);
    }
}

