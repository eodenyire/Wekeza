# PowerShell script to fix WorkflowInstance.cs issues

Write-Host "Fixing WorkflowInstance.cs..."

$filePath = "Core/Wekeza.Core.Domain/Aggregates/WorkflowInstance.cs"

if (Test-Path $filePath) {
    $content = Get-Content $filePath -Raw
    
    # Fix the ApprovalStep constructor call - remove Level parameter and add proper parameters
    $content = $content -replace 'new ApprovalStep\(\s*Level: 1,\s*AssignedTo: null,.*?\);', @'
new ApprovalStep(
            Guid.NewGuid(),
            workflow.Id,
            1,
            "APPROVER", // Default role
            null, // No specific approver
            true, // Required
            null, // No minimum amount
            null, // No maximum amount
            DateTime.UtcNow);
'@

    # Fix the second ApprovalStep constructor call
    $content = $content -replace 'new ApprovalStep\(\s*Level: CurrentLevel \+ 1,.*?\);', @'
new ApprovalStep(
                Guid.NewGuid(),
                Id,
                CurrentLevel + 1,
                "APPROVER", // Default role
                null, // No specific approver
                true, // Required
                null, // No minimum amount
                null, // No maximum amount
                DateTime.UtcNow);
'@

    # Fix the 'with' syntax usage - replace with proper method calls
    $content = $content -replace 'var updatedStep = currentStep with\s*\{[^}]*\};.*?_approvalSteps\.Remove\(currentStep\);.*?_approvalSteps\.Add\(updatedStep\);', @'
// Approve the current step
        currentStep.Approve(approvedBy, comments);'@

    # Fix the reject method 'with' syntax
    $content = $content -replace 'var updatedStep = currentStep with\s*\{[^}]*\};.*?_approvalSteps\.Remove\(currentStep\);.*?_approvalSteps\.Add\(updatedStep\);', @'
// Reject the current step
            currentStep.Reject(rejectedBy, reason);'@

    # Fix AssignToApprover method
    $content = $content -replace 'var updatedStep = step with \{ AssignedTo = approverId \};.*?_approvalSteps\.Remove\(step\);.*?_approvalSteps\.Add\(updatedStep\);', @'
// Assign the step to approver
        step.Assign();'@

    # Fix WorkflowComment constructor call
    $content = $content -replace 'new WorkflowComment\(\s*CommentBy: commentBy,\s*Comment: comment,\s*CommentDate: DateTime\.UtcNow\)', @'
new WorkflowComment(
            Guid.NewGuid(),
            Id,
            commentBy,
            comment,
            WorkflowCommentType.General,
            DateTime.UtcNow)'@

    Set-Content $filePath $content -NoNewline
    Write-Host "Fixed WorkflowInstance.cs"
} else {
    Write-Host "WorkflowInstance.cs not found"
}

Write-Host "WorkflowInstance fixes completed."