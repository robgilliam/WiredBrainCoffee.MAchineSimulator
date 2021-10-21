using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCoffee.EventHub.Sender
{
  public interface ICoffeeMachineDataSender
  {
    Task SendDataAsync(IEnumerable<CoffeeMachineData> data);
  }

  public class CoffeeMachineDataSender : ICoffeeMachineDataSender
  {
    private EventHubProducerClient _producerClient;

    private static string CONNECTION_STRING = "Endpoint=sb://rgeventhub.servicebus.windows.net/;SharedAccessKeyName=rgevents-send;SharedAccessKey=QUykB/XGS+ZRTAdJAdIGi10gGUdU96JbF1n0lyRM1vk=;EntityPath=rgevents-eh";

    private const string EVENT_HUB_NAME = "rgevents-eh";

    public CoffeeMachineDataSender()
    {
      _producerClient = new EventHubProducerClient(CONNECTION_STRING, EVENT_HUB_NAME);
    }

    public async Task SendDataAsync(IEnumerable<CoffeeMachineData> datas)
    {
      using (var eventBatch = await _producerClient.CreateBatchAsync())
      {
        foreach (var data in datas)
        {
          var dataAsJson = JsonConvert.SerializeObject(data);

          var eventData = new EventData(Encoding.UTF8.GetBytes(dataAsJson));

          if (!eventBatch.TryAdd(eventData))
          {
            throw new Exception("Bad!");
          }
        }

        await _producerClient.SendAsync(eventBatch);
      }
    }
  }
}
