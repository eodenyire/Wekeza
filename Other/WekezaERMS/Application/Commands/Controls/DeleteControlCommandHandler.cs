using MediatR;

namespace WekezaERMS.Application.Commands.Controls;

public class DeleteControlCommandHandler : IRequestHandler<DeleteControlCommand, bool>
{
    private readonly IControlRepository _controlRepository;

    public DeleteControlCommandHandler(IControlRepository controlRepository)
    {
        _controlRepository = controlRepository;
    }

    public async Task<bool> Handle(DeleteControlCommand request, CancellationToken cancellationToken)
    {
        var control = await _controlRepository.GetByIdAsync(request.Id);
        if (control == null)
        {
            return false;
        }

        await _controlRepository.DeleteAsync(control);
        await _controlRepository.SaveChangesAsync();

        return true;
    }
}
