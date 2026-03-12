# 📋 Step-by-Step Checklist: Push WekezaERMS to Separate Repository

Follow these steps to successfully push WekezaERMS to `https://github.com/eodenyire/WekezaERMS`.

## Pre-Flight Checklist

- [ ] You have access to https://github.com/eodenyire (GitHub account)
- [ ] Git is installed on your local machine
- [ ] You have cloned or can clone the Wekeza repository
- [ ] You have 5-15 minutes to complete the task

## Method Selection

Choose ONE method:

### ⭐ Option A: Automated (Linux/Mac) - RECOMMENDED
- [ ] Navigate to WekezaERMS folder
- [ ] Run: `chmod +x push-to-separate-repo.sh`
- [ ] Run: `./push-to-separate-repo.sh`
- [ ] Follow the on-screen prompts
- [ ] Choose method (recommend: 1 - Git Subtree)
- [ ] Review prepared files in working directory
- [ ] Push to GitHub when ready

### ⭐ Option B: Automated (Windows) - RECOMMENDED
- [ ] Navigate to WekezaERMS folder
- [ ] Run: `.\push-to-separate-repo.ps1`
- [ ] Follow the on-screen prompts
- [ ] Review prepared files in working directory
- [ ] Push to GitHub when ready

### Option C: Manual (5 Minutes)
- [ ] Go to GitHub and create repository: https://github.com/eodenyire/WekezaERMS
- [ ] Do NOT initialize with README (leave empty)
- [ ] Clone Wekeza repository: `git clone https://github.com/eodenyire/Wekeza.git`
- [ ] Clone new ERMS repository: `git clone https://github.com/eodenyire/WekezaERMS.git`
- [ ] Copy files: `cp -r Wekeza/WekezaERMS/* WekezaERMS/`
- [ ] Change to ERMS directory: `cd WekezaERMS`
- [ ] Stage files: `git add .`
- [ ] Commit: `git commit -m "Initial commit: WekezaERMS from Wekeza repository"`
- [ ] Push: `git push origin main`

## Verification Steps

After pushing to GitHub:

- [ ] Visit: https://github.com/eodenyire/WekezaERMS
- [ ] Verify all files are present
- [ ] Check that README.md displays correctly
- [ ] Clone the new repository locally
- [ ] Run: `cd WekezaERMS`
- [ ] Run: `dotnet restore`
- [ ] Run: `dotnet build`
- [ ] Verify: Build succeeded ✅

## Post-Setup Configuration

### On GitHub:
- [ ] Add repository description: "Wekeza Enterprise Risk Management System (ERMS)"
- [ ] Add topics: `risk-management`, `enterprise-risk`, `banking`, `dotnet`, `csharp`, `postgresql`
- [ ] Set repository visibility (Public or Private)
- [ ] Configure branch protection rules for `main` branch
- [ ] Add collaborators (if needed)
- [ ] Configure GitHub Actions (optional)

### Documentation Updates (Optional):
- [ ] Update any links that reference old repository location
- [ ] Update contact information if different from Wekeza
- [ ] Update contribution guidelines (if applicable)

## Common Issues & Solutions

### Issue: "Repository already exists"
**Solution**: Either use a different name or delete the existing repository first.

### Issue: "Permission denied"
**Solution**: Ensure you're logged in to GitHub and have push access to the repository.

### Issue: "Build failed"
**Solution**: Ensure .NET 10 SDK is installed. If using .NET 8, update TargetFramework in .csproj files.

### Issue: "Files missing after copy"
**Solution**: Use `cp -r` to copy recursively, or check that hidden files are included.

## Success Criteria

✅ Repository exists at https://github.com/eodenyire/WekezaERMS
✅ All files are present and organized correctly
✅ README displays properly on GitHub
✅ Solution builds successfully (`dotnet build`)
✅ Repository is properly configured (description, topics, etc.)

## Time Estimate

- **Automated method**: 10-15 minutes
- **Manual method**: 5-10 minutes
- **Verification**: 2-3 minutes
- **Post-setup**: 3-5 minutes
- **Total**: 15-30 minutes

## Support Resources

If you need help:

1. **Quick start**: See [QUICK-REFERENCE.md](./QUICK-REFERENCE.md)
2. **Detailed guide**: See [REPOSITORY-SETUP-GUIDE.md](./REPOSITORY-SETUP-GUIDE.md)
3. **Summary**: See [PUSH-TO-REPO-SUMMARY.md](./PUSH-TO-REPO-SUMMARY.md)
4. **Troubleshooting**: Check REPOSITORY-SETUP-GUIDE.md troubleshooting section

## Notes

- The WekezaERMS folder is self-contained and ready to move
- No code changes are needed
- All project references are relative and will work in the new repository
- The solution has been verified to build successfully

## Final Checks

Before marking complete:

- [ ] Repository is live on GitHub
- [ ] All files are present
- [ ] Build verification passed
- [ ] Documentation is accessible
- [ ] Collaborators have access (if applicable)
- [ ] You can successfully clone and build from the new repository

---

**When all items are checked**: Congratulations! 🎉 WekezaERMS is now in its own repository!

Next: Start using the ERMS API, integrate with Wekeza Core, or begin customization for your needs.
