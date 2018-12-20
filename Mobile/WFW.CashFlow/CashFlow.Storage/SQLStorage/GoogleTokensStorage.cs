using System;
using System.Linq;
using System.Threading.Tasks;
using CashFlow.Domain.Models;
using Google.Apis.Json;
using Google.Apis.Util.Store;

namespace CashFlow.Storage.SQLStorage
{
    public class GoogleTokensStorage:IDataStore
    {
        private static readonly Task CompletedTask = (Task)Task.FromResult<int>(0);
        private readonly StorageContext _context;

        public GoogleTokensStorage(StorageContext context)
        {
            _context = context;
        }

        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key MUST have a value");
            var contents = NewtonsoftJsonSerializer.Instance.Serialize((object)value);
            if (_context.Tokens.Any(t => t.Key == key))
            {
                var token = _context.Tokens.Single(t => t.Key == key);
                token.Data = contents;
            }
            else
            {
                _context.Tokens.Add(new  TokenModel
                {
                    Key = key,
                    Data =  contents
                });
            }
            _context.SaveChanges();
            return CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key MUST have a value");
            if (_context.Tokens.Any(t => t.Key == key))
            {
                var token = _context.Tokens.Single(t => t.Key == key);
                _context.Tokens.Remove(token);
                _context.SaveChanges();
            }
            return CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key MUST have a value");
            var completionSource = new TaskCompletionSource<T>();
            
            if (_context.Tokens.Any(t=>t.Key==key))
            {
                try
                {
                    var input = _context.Tokens.Single(t=>t.Key==key).Data;
                    completionSource.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(input));
                }
                catch (Exception ex)
                {
                    completionSource.SetException(ex);
                }
            }
            else
                completionSource.SetResult(default(T));
            return completionSource.Task;
        }

        public Task ClearAsync()
        {
            var items = _context.Tokens.Where(t => !t.Fixed);
            _context.Tokens.RemoveRange(items);
            _context.SaveChanges();
            return CompletedTask;
        }
    }
}
