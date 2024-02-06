using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Devris.LibDevris.MethodSinter.Tests;

public partial class FileScopedNamespaceTests
{
    private bool flag1 = false;
    private bool flag2 = false;

    [Fact]
    public void SimpleTest()
    {
        Test1_SinteredMethod();

        Assert.True(flag1);
        Assert.True(flag2);
    }

    [SinteredMethod(nameof(SimpleTest))]
    private partial void Test1_SinteredMethod();

    [MethodFragment(nameof(SimpleTest))]
    private void Test1_SetFlag1()
    {
        flag1 = true;
    }

    [MethodFragment(nameof(SimpleTest))]
    private void Test1_SetFlag2()
    {
        flag2 = true;
    }

    private readonly struct TestStruct
    {
        public TestStruct(int intValue, string stringValue)
        {
            IntValue = intValue;
            StringValue = stringValue;
        }

        readonly public int IntValue { get; init; }

        readonly public string StringValue { get; init; }
    }

    private int testStructIntValue = 0;
    private string testStructStringValue = string.Empty;
    private string stringBuilderValue = string.Empty;
    private string str1Value = string.Empty;
    private string str2Value = string.Empty;
    private string str3Value = string.Empty;

    [Fact]
    public void ComplexArgumentTest()
    {
        var testStruct = new TestStruct(3, "test");
        Test2_SinteredMethod(in testStruct, new StringBuilder("StringBuilder"), "str1", "str2", "str3");

        Assert.Equal(3, testStructIntValue);
        Assert.Equal("test", testStructStringValue);
        Assert.Equal("StringBuilder", stringBuilderValue);
        Assert.Equal("str1", str1Value);
        Assert.Equal("str2", str2Value);
        Assert.Equal("str3", str3Value);
    }

    [SinteredMethod(nameof(ComplexArgumentTest))]
    private partial void Test2_SinteredMethod(ref readonly TestStruct testStruct, [DisallowNull] StringBuilder? stringBuilder, params string[] strings);

    [MethodFragment(nameof(ComplexArgumentTest))]
    private void Test2_SetTestStruct(ref readonly TestStruct testStruct, StringBuilder? stringBuilder, IEnumerable<string> strings)
    {
        testStructIntValue = testStruct.IntValue;
        testStructStringValue = testStruct.StringValue;
    }

    [MethodFragment(nameof(ComplexArgumentTest))]
    private void Test2_SetStringBuilderValue(ref readonly TestStruct testStruct, [DisallowNull] StringBuilder? stringBuilder, params string[] strings)
    {
        stringBuilderValue = stringBuilder.ToString();
    }

    [MethodFragment(nameof(ComplexArgumentTest))]
    private void Test2_SetParamsValue(ref readonly TestStruct testStruct, StringBuilder? stringBuilder, string[] strings)
    {
        str1Value = strings[0];
        str2Value = strings[1];
        str3Value = strings[2];
    }

}