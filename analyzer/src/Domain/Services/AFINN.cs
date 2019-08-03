using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using src.Domain.Models.DataStructures;

namespace src.Domain.Services
{
    public class AFINN : IAFINN
    {
        private INodeServices _nodeServices;
        public AFINN(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }
        public void Init()
        {
            Predict("just doing this to load the node process").Wait();
        }
        public async Task<AFINNPrediction> Predict(string sentence)
        {
            var dynamicResult = await _nodeServices.InvokeAsync<dynamic>(@"./NodeScripts/afinn.js", sentence);
            return new AFINNPrediction() { Score = dynamicResult.score };
        }
    }
}