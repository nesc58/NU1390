using Newtonsoft.Json;
using NU1390.Abstracts;

namespace NU1390.Tests;

[TestFixture]
public class MessageTypeTests
{
    [Test]
    public void DeserializeJsonToMessageType_Should_DeserializePayload()
    {
        var expectedPayload = new Impl3
        {
            Field3 = "content"
        };
        var message = new Dto<Impl3> {Payload = expectedPayload};
        var payloadTypeName = typeof(Impl3).FullName;
        var messageJson = JsonConvert.SerializeObject(message);

        var deserializedMessage = MyClassHelpers.DeserializeJsonToDto<Impl3>(messageJson, payloadTypeName!);

        deserializedMessage.Payload.Should().BeEquivalentTo(expectedPayload);
    }

    [Test]
    public void DeserializeJsonToMessageType_Should_DeserializeIndexProperty()
    {
        var expectedIndex = 42;
        var payload = new Impl3();
        var message = new Dto<Impl3> {Payload = payload, Index = expectedIndex};
        var payloadTypeName = typeof(Impl3).FullName;
        var messageJson = JsonConvert.SerializeObject(message);

        var deserializedMessage = MyClassHelpers
            .DeserializeJsonToDto<Impl3>(messageJson, payloadTypeName!);

        deserializedMessage.Index.Should().Be(expectedIndex);
    }

    [Test]
    public void DeserializeJsonToMessageType_Index_Should_HaveNoValue_When_PropertyIsNotSetYet()
    {
        var messageJson = "{'Payload':{'Route':'impl3','Field3':'42'}}";
        var payloadTypeName = typeof(Impl3).FullName;

        var deserializedWebhook = MyClassHelpers.DeserializeJsonToDto<Impl3>(messageJson, payloadTypeName!);

        deserializedWebhook.Index.Should().NotHaveValue();
    }
}

public class Impl3 : MyClassBase
{
    public override string Route { get; set; } = "impl3";

    public string Field3 { get; set; } = string.Empty;
}