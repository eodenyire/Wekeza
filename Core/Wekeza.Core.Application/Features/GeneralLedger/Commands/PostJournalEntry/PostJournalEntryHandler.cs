using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.GeneralLedger.Commands.PostJournalEntry;

public class PostJournalEntryHandler : IRequestHandler<PostJournalEntryCommand, string>
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PostJournalEntryHandler(
        IJournalEntryRepository journalRepository,
        IGLAccountRepository glAccountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _journalRepository = journalRepository;
        _glAccountRepository = glAccountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(PostJournalEntryCommand request, CancellationToken cancellationToken)
    {
        // Generate journal number
        var journalNumber = await _journalRepository.GenerateJournalNumberAsync(cancellationToken);

        // Create journal entry
        var journal = JournalEntry.Create(
            journalNumber,
            request.PostingDate,
            request.ValueDate,
            request.Type,
            request.SourceType,
            request.SourceId,
            request.SourceReference,
            request.Currency,
            (_currentUserService.UserId ?? Guid.Empty).ToString(),
            request.Description);

        // Add lines
        foreach (var lineDto in request.Lines)
        {
            // Validate GL account exists
            var glAccount = await _glAccountRepository.GetByGLCodeAsync(lineDto.GLCode, cancellationToken);
            if (glAccount == null)
            {
                throw new InvalidOperationException($"GL account {lineDto.GLCode} not found.");
            }

            if (!glAccount.IsLeaf)
            {
                throw new InvalidOperationException($"Cannot post to non-leaf GL account {lineDto.GLCode}.");
            }

            journal.AddLine(new JournalLine(
                LineNumber: journal.Lines.Count + 1,
                GLCode: lineDto.GLCode,
                DebitAmount: lineDto.DebitAmount,
                CreditAmount: lineDto.CreditAmount,
                CostCenter: lineDto.CostCenter,
                ProfitCenter: lineDto.ProfitCenter,
                Description: lineDto.Description));
        }

        // Post journal entry
        journal.Post((_currentUserService.UserId ?? Guid.Empty).ToString());

        // Update GL account balances
        foreach (var line in journal.Lines)
        {
            var glAccount = await _glAccountRepository.GetByGLCodeAsync(line.GLCode, cancellationToken);
            if (glAccount != null)
            {
                if (line.DebitAmount > 0)
                    glAccount.PostDebit(line.DebitAmount);
                if (line.CreditAmount > 0)
                    glAccount.PostCredit(line.CreditAmount);
            }
        }

        // Save
        await _journalRepository.AddAsync(journal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return journalNumber;
    }
}
