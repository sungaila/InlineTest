# ![InlineTest Logo](https://raw.githubusercontent.com/sungaila/InlineTest/master/etc/Icon_64.png) Sungaila.InlineTest

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

## Example
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
// shortened code for readability
[GeneratedCode("Sungaila.InlineTest", "1.0.0+17ac90a4b0b471c88edc5fcedee4124a7cbbac28")]
[TestClass]
public partial class ReadmeExampleTests
{
	[TestMethod]
	[DataRow(6, 3, 2)]
	[DataRow(1, 1, 1)]
	public void DivideAreEqual(int dividend, int divisor, int expected)
	{
		var result = ReadmeExample.Divide(dividend, divisor);
		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow(10, 1, 42)]
	public void DivideAreNotEqual(int dividend, int divisor, int notExpected)
	{
		var result = ReadmeExample.Divide(dividend, divisor);
		Assert.AreNotEqual(notExpected, result);
	}

	[TestMethod]
	[DataRow(1, 0)]
	public void DivideThrowsException_ArgumentOutOfRangeException(int dividend, int divisor)
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(
			() => ReadmeExample.Divide(dividend, divisor));
	}
}
```

## Restrictions
1. The method must be defined inside a `class` or `struct`.
    - These must be `public` or `internal`
    - For classes without a parameterless constructor the method must be static
2. The method must follow the rules of a test method (MSTest).
    - must be `public`
    - cannot be `async void`
    - cannot use generics
3. The method must not have more than 15 parameters.
4. The same [Attribute](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/attributes) rules apply here. Your parameters have to be either
    - a constant value
    - a `System.Type` (defined at compile-time)
    - a single-dimensional array of the above
