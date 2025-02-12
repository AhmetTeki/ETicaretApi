using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.ProductImageFile;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowcaseImage
{
    public class ChangeShowcaseImageCommandHandler : IRequestHandler<ChangeShowcaseImageCommandRequest, ChangeShowcaseImageCommandResponse>
    {
       readonly IProductImageFileWriteRepository productImageFileWriteRepository;

        public ChangeShowcaseImageCommandHandler(IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            this.productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<ChangeShowcaseImageCommandResponse> Handle(ChangeShowcaseImageCommandRequest request, CancellationToken cancellationToken)
        {
            var query =  productImageFileWriteRepository.Table.Include(p => p.Products).SelectMany(p => p.Products, (pif, p) => new {
                pif,
                p
            });
           var data= await query.FirstOrDefaultAsync(p=>p.p.Id== Guid.Parse(request.productId)&& p.pif.Showcase);

            if (data != null)
                data.pif.Showcase = false;
            var image = await query.FirstOrDefaultAsync(p => p.pif.Id == Guid.Parse(request.imageId));
            if (image != null)
                image.pif.Showcase = true;
          await  productImageFileWriteRepository.SaveAsync();

            return new ();

        }
    }
}
