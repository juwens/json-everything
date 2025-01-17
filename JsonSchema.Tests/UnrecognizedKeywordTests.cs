﻿using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using NUnit.Framework;

namespace Json.Schema.Tests;

public class UnrecognizedKeywordTests
{
	[Test]
	public void FooIsNotAKeyword()
	{
		var schemaText = "{\"foo\": \"bar\"}";

		var schema = JsonSerializer.Deserialize<JsonSchema>(schemaText);

		Assert.AreEqual(1, schema!.Keywords!.Count);
		Assert.IsInstanceOf<UnrecognizedKeyword>(schema.Keywords.First());
	}

	[Test]
	public void FooProducesAnAnnotation()
	{
		var schemaText = "{\"foo\": \"bar\"}";

		var schema = JsonSerializer.Deserialize<JsonSchema>(schemaText);

		var result = schema!.Evaluate(new JsonObject(), new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });

		Assert.IsTrue(result.IsValid);
		Assert.AreEqual(1, result.Annotations!.Count);
		Assert.IsTrue(((JsonNode?)"bar").IsEquivalentTo(result.Annotations.First().Value));
	}

	[Test]
	public void FooProducesAnAnnotation_Constructed()
	{
		var schema = new JsonSchemaBuilder()
			.Unrecognized("foo", "bar")
			.Build();

		var result = schema.Evaluate(new JsonObject(), new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });

		Assert.IsTrue(result.IsValid);
		Assert.AreEqual(1, result.Annotations!.Count);
		Assert.IsTrue(((JsonNode?)"bar").IsEquivalentTo(result.Annotations.First().Value));
	}

	[Test]
	public void FooProducesAnAnnotation_WithSchemaKeyword()
	{
		var options = new EvaluationOptions
		{
			OutputFormat = OutputFormat.Hierarchical
		};
		var schema = new JsonSchemaBuilder()
			.Id("https://example.com")
			.Schema(MetaSchemas.Draft202012Id)
			.Unrecognized("foo", "bar")
			.Build();

		var instance = new JsonObject();

		var result = schema.Evaluate(instance, options);

		Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		}));

		Assert.IsTrue(result.IsValid);
		Assert.AreEqual(1, result.Annotations!.Count);
		Assert.IsTrue(((JsonNode?)"bar").IsEquivalentTo(result.Annotations.First().Value));
	}

	[Test]
	public void FooIsIncludedInSerialization()
	{
		var schemaText = "{\"foo\":\"bar\"}";

		var schema = JsonSerializer.Deserialize<JsonSchema>(schemaText);

		var reText = JsonSerializer.Serialize(schema);

		Assert.AreEqual(schemaText, reText);
	}
}