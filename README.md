Replaced by [https://github.com/samsmithnz/DotnetCensus/actions](https://github.com/samsmithnz/DotnetCensus/actions)

# TechDebtID
The exploratory project aims to understand if we can identify, classify and rank technical debt in repos in a way that would be useful

what if... we can help to tackle technical debt? The first stage is to identify the technical debt.

![Current build](https://github.com/samsmithnz/TechDebtIdentification/workflows/Technical%20Debt%20ID%20CI/CD/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/samsmithnz/TechDebtIdentification/badge.svg?branch=main)](https://coveralls.io/github/samsmithnz/TechDebtIdentification?branch=main)




## Usage
```
techid.exe -f "c:\repos" -i true -o "c:\results\results.csv"
```

### Parameters
```
  -f, --folder:	Required. Root local folder to search for projects, e.g. c:\repos

  -t, --includetotals: (Default is false) Include totals in results

  -o, --output: output file to create csv file; e.g. c:\results\results.csv

  -g, --githubOrg: GitHub org to scan; e.g. samsmithnz 
```


### Sample output:
```
Project files found: 223
======================================
Framework         FrameworkFamily  Count
----------------------------------------
net45             .NET Framework       1
net5.0            .NET                30
net5.0-windows    .NET                 3
net6.0            .NET                 1
netcoreapp2.0     .NET Core            1
netcoreapp2.1     .NET Core           25
netcoreapp2.2     .NET Core            3
netcoreapp3.0     .NET Core           11
netcoreapp3.1     .NET Core           35
netstandard2.0    .NET Standard       13
netstandard2.1    .NET Standard        5
v1.0              .NET Framework       2
v1.1              .NET Framework       3
v2.0              .NET Framework       6
v3.5              .NET Framework      21
v4.0              .NET Framework       4
v4.5              .NET Framework       4
v4.5.2            .NET Framework       5
v4.6              .NET Framework       2
v4.6.1            .NET Framework      33
v4.7.1            .NET Framework       9
v4.7.2            .NET Framework       6
vb6               Visual Basic 6       1
total frameworks                     223
unique frameworks:                    22

======================================
Language          Count
-----------------------
csharp              205
vb.net               17
vb6                   1
total languages:    223
unique languages:     3
```
