﻿/* Copyright 2015 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace MongoDB.Driver.GridFS.Tests
{
    [TestFixture]
    public class GridFSFileInfoTests
    {
        [Test]
        public void Aliases_get_should_return_the_expected_result()
        {
            var value = new[] { "alias" };
            var subject = CreateSubject(aliases: value);

#pragma warning disable 618
            var result = subject.Aliases;
#pragma warning restore

            result.Should().Equal(value);
        }

        [Test]
        public void Aliases_should_be_deserialized_correctly(
            [Values(null, new string[0], new string[] { null }, new[] { "a" }, new[] { "a", "b" })]
            string[] value)
        {
            var document = CreateFilesCollectionDocument();
            if (value != null)
            {
                document["aliases"] = new BsonArray(value);
            }

            var subject = DeserializeFilesCollectionDocument(document);

#pragma warning disable 618
            subject.Aliases.Should().Equal(value);
#pragma warning restore
        }

        [Test]
        public void ChunkSizeBytes_get_should_return_the_expected_result()
        {
            var value = 1024;
            var subject = CreateSubject(chunkSizeBytes: value);

            var result = subject.ChunkSizeBytes;

            result.Should().Be(value);
        }

        [Test]
        public void ChunkSizeBytes_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var subject = DeserializeFilesCollectionDocument(document);

            subject.ChunkSizeBytes.Should().Be(document["chunkSize"].ToInt32());
        }

        [Test]
        public void constructor_should_initialize_instance()
        {
#pragma warning disable 618
            var aliases = new[] { "alias" };
            var chunkSizeBytes = 1024;
            var contentType = "type";
            var extraElements = new BsonDocument();
            var filename = "name";
            var id = ObjectId.GenerateNewId();
            var idAsBsonValue = (BsonValue)id;
            var length = 512;
            var md5 = "md5";
            var metadata = new BsonDocument();
            var uploadDateTime = DateTime.UtcNow;

            var result = new GridFSFileInfo(
                aliases,
                chunkSizeBytes,
                contentType,
                extraElements,
                filename,
                idAsBsonValue,
                length,
                md5,
                metadata,
                uploadDateTime);

            result.Aliases.Should().BeSameAs(aliases);
            result.ChunkSizeBytes.Should().Be(chunkSizeBytes);
            result.ContentType.Should().Be(contentType);
            result.ExtraElements.Should().Be(extraElements);
            result.Filename.Should().Be(filename);
            result.Id.Should().Be(id);
            result.IdAsBsonValue.Should().Be(idAsBsonValue);
            result.Length.Should().Be(length);
            result.MD5.Should().Be(md5);
            result.Metadata.Should().Be(metadata);
            result.UploadDateTime.Should().Be(uploadDateTime);
#pragma warning restore
        }

        [Test]
        public void ContentType_get_should_return_the_expected_result()
        {
            var value = "application/image";
            var subject = CreateSubject(contentType: value);

#pragma warning disable 618
            var result = subject.ContentType;
#pragma warning restore

            result.Should().Be(value);
        }

        [Test]
        public void ContentType_should_be_deserialized_correctly(
            [Values(null, "type")]
            string value)
        {
            var document = CreateFilesCollectionDocument();
            if (value != null)
            {
                document["contentType"] = value;
            }

            var subject = DeserializeFilesCollectionDocument(document);

#pragma warning disable 618
            subject.ContentType.Should().Be(value);
#pragma warning restore
        }

        [Test]
        public void ExtraElements_get_should_return_the_expected_result()
        {
            var value = new BsonDocument("x", 1);
            var subject = CreateSubject(extraElements: value);

            var result = subject.ExtraElements;

            result.Should().Be(value);
        }

        [Test]
        public void ExtraElements_should_be_deserialized_correctly(
            [Values(null, new[] { "x" }, new[] { "x", "y" }, new[] { "ExtraElements" })]
            string[] names)
        {
            var document = CreateFilesCollectionDocument();

            BsonDocument extraElements = null;
            if (names != null)
            {
                extraElements = new BsonDocument();
                var value = 1;
                foreach (var name in names)
                {
                    extraElements.Add(name, value++);
                }

                document.Merge(extraElements);
            }

            var subject = DeserializeFilesCollectionDocument(document);

            subject.ExtraElements.Should().Be(extraElements);
        }

        [Test]
        public void Filename_get_should_return_the_expected_result()
        {
            var value = "abc";
            var subject = CreateSubject(filename: value);

            var result = subject.Filename;

            result.Should().Be(value);
        }

        [Test]
        public void Filename_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var result = DeserializeFilesCollectionDocument(document);

            result.Filename.Should().Be(document["filename"].AsString);
        }

        [Test]
        public void Id_get_should_return_the_expected_result()
        {
            var value = ObjectId.GenerateNewId();
            var subject = CreateSubject(idAsBsonValue: value);

            var result = subject.Id;

            result.Should().Be(value);
        }

        [Test]
        public void Id_get_should_throw_when_id_is_not_an_ObjectId()
        {
            var value = (BsonValue)123;
            var subject = CreateSubject(idAsBsonValue: value);

            Action action = () => { var id = subject.Id; };

            action.ShouldThrow<InvalidCastException>();
        }

        [Test]
        public void Id_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var result = DeserializeFilesCollectionDocument(document);

            result.Id.Should().Be(document["_id"].AsObjectId);
#pragma warning disable 618
            result.IdAsBsonValue.Should().Be(document["_id"]);
#pragma warning restore
        }

        [Test]
        public void Id_should_be_deserialized_correctly_when_id_is_not_an_ObjectId()
        {
            var document = CreateFilesCollectionDocument();
            document["_id"] = 123;

            var result = DeserializeFilesCollectionDocument(document);

#pragma warning disable 618
            result.IdAsBsonValue.Should().Be(document["_id"]);
#pragma warning restore
        }

        [Test]
        public void IdAsBsonValue_get_should_return_the_expected_result()
        {
            var value = (BsonValue)123;
            var subject = CreateSubject(idAsBsonValue: value);

#pragma warning disable 618
            var result = subject.IdAsBsonValue;
#pragma warning restore

            result.Should().Be(value);
        }

        [Test]
        public void Length_get_should_return_the_expected_result()
        {
            var value = 123;
            var subject = CreateSubject(length: value);

            var result = subject.Length;

            result.Should().Be(value);
        }

        [Test]
        public void Length_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var subject = DeserializeFilesCollectionDocument(document);

            subject.Length.Should().Be(document["length"].ToInt64());
        }

        [Test]
        public void MD5_get_should_return_the_expected_result()
        {
            var value = "md5";
            var subject = CreateSubject(md5: value);

            var result = subject.MD5;

            result.Should().Be(value);
        }

        [Test]
        public void MD5_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var subject = DeserializeFilesCollectionDocument(document);

            subject.MD5.Should().Be(document["md5"].AsString);
        }

        [Test]
        public void Metadata_get_should_return_the_expected_result()
        {
            var value = new BsonDocument("x", 1);
            var subject = CreateSubject(metadata: value);

            var result = subject.Metadata;

            result.Should().Be(value);
        }

        [Test]
        public void Metadata_should_be_deserialized_correctly(
            [Values(null, "{ }", "{ x : 1 }")]
            string json)
        {
            var document = CreateFilesCollectionDocument();
            BsonDocument metadata = null;
            if (json != null)
            {
                metadata = BsonDocument.Parse(json);
                document["metadata"] = metadata;
            }

            var result = DeserializeFilesCollectionDocument(document);

            result.Metadata.Should().Be(metadata);
        }

        [Test]
        public void UploadDateTime_get_should_return_the_expected_result()
        {
            var value = DateTime.UtcNow;
            var subject = CreateSubject(uploadDateTime: value);

            var result = subject.UploadDateTime;

            result.Should().Be(value);
        }

        [Test]
        public void UploadDateTime_should_be_deserialized_correctly()
        {
            var document = CreateFilesCollectionDocument();

            var subject = DeserializeFilesCollectionDocument(document);

            subject.UploadDateTime.Should().Be(document["uploadDate"].ToUniversalTime());
        }

        // private methods
        private BsonDocument CreateFilesCollectionDocument()
        {
            return new BsonDocument
            {
                { "_id", ObjectId.GenerateNewId() },
                { "length", 123 },
                { "chunkSize", 1024 },
                { "uploadDate", DateTime.UtcNow },
                { "md5", "md5" },
                { "filename", "name" }
            };
        }

        private GridFSFileInfo CreateSubject(
             IEnumerable<string> aliases = null,
            int? chunkSizeBytes = null,
            string contentType = null,
            BsonDocument extraElements = null,
            string filename = null,
            BsonValue idAsBsonValue = null,
            long? length = null,
            string md5 = null,
            BsonDocument metadata = null,
            DateTime? uploadDateTime = null)
        {
            return new GridFSFileInfo(
                aliases,
                chunkSizeBytes ?? 255 * 1024,
                contentType,
                extraElements,
                filename ?? "name",
                idAsBsonValue ?? (BsonValue)ObjectId.GenerateNewId(),
                length ?? 0,
                md5 ?? "md5",
                metadata,
                uploadDateTime ?? DateTime.UtcNow);
        }

        private GridFSFileInfo DeserializeFilesCollectionDocument(BsonDocument document)
        {
            return BsonSerializer.Deserialize<GridFSFileInfo>(document);
        }
    }
}