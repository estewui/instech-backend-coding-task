# Description
This project is a backend coding task realised by me as part of my application to InsuranceTechnologySolutions.  
**Author**: Stanislaw Teczynski  
**Date**: 2026/02/23

# Prerequisites
- Visual Studio, minimal version v17.12
- Running Docker engine

# Instructions
1. Clone the repository: `git clone git@github.com:estewui/instech-backend-coding-task.git`
2. Open file Claims.sln in Visual Studio
3. In top navigation bar, run the project API.
4. Trust the certificate if Visual Studio asks you.
5. New browser window with Swagger will open up. You can test API there. 

# Tests
1. On the navigation bar on the right, right-click on Claims.Tests project.
2. Click *Run Tests*

# Note
Regarding Task 5 (Cover premium computation) - Instructions on github are a little bit unclear to me. Days *31-180* are discounted by `{X}%`, and days *181+* are discounted by an **_ADDITIONAL_** `{Y}%`
My concern is, it's not explicitally explained if the second discount `Y %` should be applied to the initial base rate, or to the 'middle' base rate from days *31-180*.  
For example, if initial base day rate would be *100$*, `X=2%`, `Y=1%`, `base multiplier = 1` then:
- **scenario A** (discount applied to the initial base rate) - price for day *31* would be `100 * (1 - 0.02) = 98$`, and price for day *181* would be `100 * (1 - (0.02 + 0.01)) = 97$`
- **scenario B** (discount applied to the 'middle' base rate) - price for day *31* would be `100 * (1 - 0.02) = 98$`, and price for day *181* would be `98 * (1 - 0.01) = 97.02$`

In my solution, I decided to choose scenario **A**, as it was already implemented in the code before and seems more straightforward.
In case I would like to implement scenario **B**, I would only adjust one line: `var after150DaysRate = after30daysBefore180DaysRate * (1.00m - (coverMultiplier.After150DaysDiscount));` in *Claims\Domain\Services\PremiumCalculator.cs*

