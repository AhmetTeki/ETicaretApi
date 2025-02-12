using ETicaretAPI.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Basket.UpdateQuentity
{
    public class UpdateQuentityCommandHandler : IRequestHandler<UpdateQuentityCommandRequest, UpdateQuentityCommandResponse>
    {
        readonly IBasketService _basketService;

        public UpdateQuentityCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<UpdateQuentityCommandResponse> Handle(UpdateQuentityCommandRequest request, CancellationToken cancellationToken)
        {
          await _basketService.UpdateQuantityAsync(new()
          {
              BasketItemId=request.BasketItemId,
              Quentity=request.Quentity,
          });
            return new();
        }
    }
}
