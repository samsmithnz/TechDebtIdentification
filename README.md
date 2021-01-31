# TechDebtIdentification
The exploratory project aims to understand if we can identify, classify and rank technical debt in repos in a way that would be useful

what if... we can help to tackle technical debt?

**The first stage is identification**

## Usage
```
techid.exe -f "c:\repos" -t true -o "c:\results\results.csv"
```

### Parameters
```
  -f, --folder    Required. Root folder to search for projects

  -t, --totals    (Default: false) Include totals in results

  -o, --output    output file to create csv file
```
