# TechDebtIdentification
The exploratory project aims to understand if we can identify, classify and rank technical debt in repos in a way that would be useful

what if... we can help to tackle technical debt?

![Current build](https://github.com/samsmithnz/TechDebtIdentification/workflows/CI/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/samsmithnz/TechDebtIdentification/badge.svg?branch=main)](https://coveralls.io/github/samsmithnz/TechDebtIdentification?branch=main)
![Current Release](https://img.shields.io/github/release/samsmithnz/TechDebtIdentification/all.svg)

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
