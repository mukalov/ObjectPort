# Benchmarks
All benchmarks were made using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) library. The code of benchmark tests is avialable in the [repository](src/ObjectPort.Benchmarks). The following binary serialized were used in comparison benchmarks:

* [NetSerializer](https://github.com/tomba/netserializer)
* [MessageShark](https://github.com/rpgmaker/MessageShark)
* [Salar.Bois](https://github.com/salarcode/Bois)
* [protobuf-net](https://github.com/mgravell/protobuf-net)
* [Wire](https://github.com/rogeralsing/Wire)

## Serialization benchmark

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4510U CPU 2.00GHz, ProcessorCount=4
Frequency=2533199 ticks, Resolution=394.7578 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=SimpleSerializationBenchmarks  Mode=Throughput  

```
        Method | Toolchain | Runtime |    Median |    StdDev |
-------------- |---------- |-------- |---------- |---------- |
 NetSerializer |       Clr |     Clr | 5.2740 us | 0.3066 us |
  MessageShark |       Clr |     Clr | 5.1514 us | 0.3567 us |
         Salar |       Clr |     Clr | 6.8791 us | 0.1580 us |
      Protobuf |       Clr |     Clr | 4.3132 us | 0.3038 us |
          Wire |       Clr |     Clr | 9.0885 us | 0.6393 us |
    ObjectPort |       Clr |     Clr | 3.9928 us | 0.3496 us |
 NetSerializer |      Core |    Core |        NA |        NA |
  MessageShark |      Core |    Core |        NA |        NA |
         Salar |      Core |    Core |        NA |        NA |
      Protobuf |      Core |    Core | 4.3020 us | 0.3528 us |
          Wire |      Core |    Core | 8.2315 us | 0.7391 us |
    ObjectPort |      Core |    Core | 3.7383 us | 0.7417 us |

Benchmarks with issues:
  SimpleSerializationBenchmarks_NetSerializer_Core
  SimpleSerializationBenchmarks_MessageShark_Core
  SimpleSerializationBenchmarks_Salar_Core


## Deserialization benchmark

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4510U CPU 2.00GHz, ProcessorCount=4
Frequency=2533199 ticks, Resolution=394.7578 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=SimpleDeserializationBenchmarks  Mode=Throughput  

```
        Method | Toolchain | Runtime |     Median |    StdDev |
-------------- |---------- |-------- |----------- |---------- |
 NetSerializer |       Clr |     Clr |  6.8457 us | 0.5145 us |
  MessageShark |       Clr |     Clr |  5.1667 us | 0.2560 us |
     SalarBois |       Clr |     Clr |  9.1745 us | 0.6016 us |
      Protobuf |       Clr |     Clr |  9.2682 us | 0.7218 us |
          Wire |       Clr |     Clr | 10.3036 us | 0.6854 us |
    ObjectPort |       Clr |     Clr |  5.1964 us | 0.4530 us |
 NetSerializer |      Core |    Core |         NA |        NA |
  MessageShark |      Core |    Core |         NA |        NA |
     SalarBois |      Core |    Core |         NA |        NA |
      Protobuf |      Core |    Core | 10.1938 us | 1.5606 us |
          Wire |      Core |    Core |  9.5938 us | 0.4698 us |
    ObjectPort |      Core |    Core |  3.8877 us | 0.4355 us |

Benchmarks with issues:
  SimpleDeserializationBenchmarks_NetSerializer_Core
  SimpleDeserializationBenchmarks_MessageShark_Core
  SimpleDeserializationBenchmarks_SalarBois_Core
