using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Basket.UpdateQuentity
{
    public class UpdateQuentityCommandRequest:IRequest<UpdateQuentityCommandResponse>
    {
        public string BasketItemId { get; set; }
        public int Quentity { get; set; }
    }
}