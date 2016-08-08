#region

using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Mappers.Tests
{
    public class SimpleMapperTests
    {
        [Test]
        public void Map_SourceToDestination()
        {
            var source = new MapSource
            {
                ShouldNotMap_DiffPropName = "ShouldNotMap_DiffPropName",
                ShouldNotMap_DiffPropType = "ShouldNotMap_DiffPropName",
                ShouldNotMap_NotExists = "ShouldNotMap_NotExists",
                String = "String",
                Sub = new Source_SubType
                {
                    Int = int.MaxValue,
                    Long = long.MaxValue
                },
                Should_Map_From_Int_To_TestEnum = 1,
                Should_Map_From_TestEnum_To_Int = TestEnum.Value2
            };

            var dest = SimpleMapper.Map<MapSource, MapDestination>(source);

            dest.String.ShouldEqual(source.String);
            dest.ShouldNotMap_DiffPropName_.ShouldEqual(default(string));
            dest.Sub.ShouldEqual(source.Sub);
            dest.ShouldNotMap_DiffPropType.ShouldEqual(default(int));
            dest.Should_Map_From_Int_To_TestEnum.ShouldEqual((TestEnum) source.Should_Map_From_Int_To_TestEnum);
            dest.Should_Map_From_TestEnum_To_Int.ShouldEqual((int) source.Should_Map_From_TestEnum_To_Int);
        }

        [Test]
        public void Map_SourceInstanceToDestinationInstance()
        {
            var source = new MapSource
            {
                ShouldNotMap_DiffPropName = "ShouldNotMap_DiffPropName",
                ShouldNotMap_DiffPropType = "ShouldNotMap_DiffPropName",
                ShouldNotMap_NotExists = "ShouldNotMap_NotExists",
                String = "String",
                Sub = new Source_SubType
                {
                    Int = int.MaxValue,
                    Long = long.MaxValue
                },
                Should_Map_From_Int_To_TestEnum = 1,
                Should_Map_From_TestEnum_To_Int = TestEnum.Value2
            };

            var dest = new MapDestination
            {
                String = "Thgis is init Value",
                Should_Map_From_Int_To_TestEnum = TestEnum.Value2,
                Should_Map_From_TestEnum_To_Int = int.MaxValue,
            };

            SimpleMapper.Map(source, dest);

            dest.String.ShouldEqual(source.String);
            dest.ShouldNotMap_DiffPropName_.ShouldEqual(default(string));
            dest.Sub.ShouldEqual(source.Sub);
            dest.ShouldNotMap_DiffPropType.ShouldEqual(default(int));
            dest.Should_Map_From_Int_To_TestEnum.ShouldEqual((TestEnum) source.Should_Map_From_Int_To_TestEnum);
            dest.Should_Map_From_TestEnum_To_Int.ShouldEqual((int) source.Should_Map_From_TestEnum_To_Int);
        }



        public enum TestEnum
        {
            Value1 = 1,
            Value2 = 2
        };

        public class MapSource
        {
            public string String { get; set; }

            public string ShouldNotMap_NotExists { get; set; }

            public string ShouldNotMap_DiffPropName { get; set; }

            public string ShouldNotMap_DiffPropType { get; set; }


            public Source_SubType Sub { get; set; }
            public int Should_Map_From_Int_To_TestEnum { get; set; }

            public TestEnum Should_Map_From_TestEnum_To_Int { get; set; }
        }

        public class Source_SubType
        {
            public int Int { get; set; }

            public long Long { get; set; }
        }

        public class MapDestination
        {
            public TestEnum Should_Map_From_Int_To_TestEnum { get; set; }

            public int Should_Map_From_TestEnum_To_Int { get; set; }

            public string String { get; set; }

            public Source_SubType Sub { get; set; }

            public string ShouldNotMap_DiffPropName_ { get; set; }

            public int ShouldNotMap_DiffPropType { get; set; }
        }
    }
}