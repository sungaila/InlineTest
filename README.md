# <img src="https://raw.githubusercontent.com/sungaila/InlineTest/master/etc/Icon.svg" width="64" height="64" alt="InlineTest Logo"> InlineTest

[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/sungaila/b346bdfc-2251-46e5-82b0-fa1153cea3eb/6/master?style=flat-square)](https://dev.azure.com/sungaila/InlineTest/_build/latest?definitionId=6&branchName=master)
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/sungaila/InlineTest/6/master?style=flat-square)](https://dev.azure.com/sungaila/InlineTest/_build/latest?definitionId=6&branchName=master)
[![SonarCloud Quality Gate](https://img.shields.io/sonar/quality_gate/sungaila_InlineTest?server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/dashboard?id=sungaila_InlineTest)
[![NuGet version](https://img.shields.io/nuget/v/Sungaila.InlineTest.svg?style=flat-square)](https://www.nuget.org/packages/Sungaila.InlineTest/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Sungaila.InlineTest.svg?style=flat-square)](https://www.nuget.org/packages/Sungaila.InlineTest/)
[![GitHub license](https://img.shields.io/github/license/sungaila/InlineTest?style=flat-square)](https://github.com/sungaila/InlineTest/blob/master/LICENSE)


A [C# source generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) for quick creation of simple unit tests. Just add these attributes to your method:
* `[AreEqual]`
* `[AreNotEqual]`
* `[IsTrue]`
* `[IsFalse]`
* `[IsNull]`
* `[IsNotNull]`
* `[IsInstanceOfType<T>]`
* `[IsNotInstanceOfType<T>]`
* `[ThrowsException<T>]`

```csharp
[AreEqual(6, 3, Expected = 2)]
[AreEqual(1, 1, Expected = 1)]
[AreNotEqual(10, 1, NotExpected = 42)]
[ThrowsException<ArgumentOutOfRangeException>(1, 0)]
public static int Divide(int dividend, int divisor)
{
	if (divisor == 0)
		throw new ArgumentOutOfRangeException(nameof(divisor));

	return dividend / divisor;
}
```

The source generator will produce classes containing the matching unit tests.

```csharp
[GeneratedCode("Sungaila.InlineTest", "0.9.0-debug+17ac90a4b0b471c88edc5fcedee4124a7cbbac28")]
[TestClass]
public partial class ReadmeExampleTests
{
	[TestMethod]
	[DataRow(6, 3, 2, DisplayName = "Divide(dividend: 6, divisor: 3) is 2")]
	[DataRow(1, 1, 1, DisplayName = "Divide(dividend: 1, divisor: 1) is 1")]
	public void DivideAreEqual(int dividend, int divisor, int expected)
	{
		var result = ReadmeExample.Divide(dividend, divisor);
		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow(10, 1, 42, DisplayName = "Divide(dividend: 10, divisor: 1) is not 42")]
	public void DivideAreNotEqual(int dividend, int divisor, int notExpected)
	{
		var result = ReadmeExample.Divide(dividend, divisor);
		Assert.AreNotEqual(notExpected, result);
	}

	[TestMethod]
	[DataRow(1, 0, DisplayName = "Divide(dividend: 1, divisor: 0) throws ArgumentOutOfRangeException")]
	public void DivideThrowsException_System_ArgumentOutOfRangeException(int dividend, int divisor)
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ReadmeExample.Divide(dividend, divisor));
	}
}
```

## Restrictions
1. The same Attribute rules apply here. Your parameters have to be either
    - a constant value
    - a `System.Type`
    - a single-dimensional array of the above
2. The annotated method must be `static` or the declaring class must have a parameterless constructor.
3. The annotated method must not have more than 16 parameters.
