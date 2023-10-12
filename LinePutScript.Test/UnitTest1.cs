using System;
using System.Text;
using NUnit.Framework;

namespace LinePutScript.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreateDocumentAndConvertToString1()
    {
        var compareText = new StringBuilder()
            .AppendLine("money#10500:|")
            .AppendLine("computer:|name#我的电脑:|")
            .ToString()
            .TrimEnd('\n');

        var document = new LpsDocument
        {
            [(gint)"money"] = 10000,
            ["computer"] = { [(gstr)"name"] = "我的电脑" }
        };
        document[(gint)"money"] += 500;
        var result = document.ToString();
        
        Assert.That(compareText, Is.EqualTo(result));
    }
    
    [Test]
    public void Test_CreateDocumentAndConvertToString2()
    {
        var compareText = new StringBuilder()
            .AppendLine("money#10500:|")
            .AppendLine("computer:|name#我的电脑:|")
            .ToString()
            .TrimEnd('\n');

        var document = new LpsDocument
        {
            [(gint)"money"] = 10000,
            ["computer"] =
            {
                ["name"] =
                {
                    Value = "我的电脑"
                }
            }
        };
        document[(gint)"money"] += 500;
        var result = document.ToString();
        
        Assert.That(compareText, Is.EqualTo(result));
    }

    [Test]
    public void Test_CreateDocumentAndConvertToString3()
    {
        var compareText = new StringBuilder()
            .AppendLine("player:|name#Thomas1000:|level#3:|")
            .ToString()
            .TrimEnd('\n');

        var document = new LpsDocument
        {
            ["player"] =
            {
                ["name"] = {
                    Value = "Thomas1000"
                },
                ["level"] = {
                    Value = "3"
                }
            }
        };
        var result = document.ToString();
        
        Assert.That(compareText, Is.EqualTo(result));
    }
    
    [Test]
    public void Test_CreateDocumentAndSave1()
    {
        const string msgStr = "Hi sweetie! <3";
        const bool isTrueLove = true;
        const int minInt = int.MinValue;
        const float floatValue = 6969.69f;
        const double piValue = Math.PI;

        var serialised = new LpsDocument
        {
            [(gstr)nameof(msgStr)] = msgStr,
            [(gint)nameof(minInt)] = minInt,
            [(gflt)nameof(floatValue)] = floatValue,
            [(gdbe)nameof(piValue)] = piValue,
            [(gbol)nameof(isTrueLove)] = isTrueLove
        }.ToString();
        
        var document = new LpsDocument(serialised);
        
        Assert.Multiple(() =>
        {
            Assert.That(document.GetString(nameof(msgStr)), Is.EqualTo(msgStr));
            Assert.That(document.GetBool(nameof(isTrueLove)), Is.EqualTo(isTrueLove));
            Assert.That(document.GetInt(nameof(minInt)), Is.EqualTo(minInt));
            Assert.That(document.GetFloat(nameof(floatValue)), Is.EqualTo(floatValue));
            Assert.That(document.GetDouble(nameof(piValue)), Is.EqualTo(piValue));
        });
    }
}