using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;

namespace Cloudsdale.Controllers.Login {
    public interface ILoginProcessor {
        Task<LoggedInUser> Login(IDictionary<string, string> data);
        int ErrorCode { get; }
    }
}
