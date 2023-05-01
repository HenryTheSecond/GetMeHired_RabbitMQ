using Producer.Dto;

namespace Producer.RabbitMQService
{
    public interface IJobSearchService
    {
        void PublishMessage(JobQueryRequest request);
    }
}
