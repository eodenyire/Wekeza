using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Controls;

public class GetControlByIdQuery : IRequest<ControlDto?>
{
    public Guid Id { get; set; }
}
