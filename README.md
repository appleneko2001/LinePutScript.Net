# LinePutScript.Net

LinePutScript is a library, used for serialise and deserialise the LinePutScript file format, which developed by 
github@LorisYounger. Its some sort of csv like, but more complex and probably its used for proprietary projects from LorisYounger (or 深圳市零版计算机技术有限公司) AFAIK.    

LinePutScript.Net is LinePutScript, but with some typo corrections and change target framework to .netstandard
instead of .net framework and .net due to the limitations of not using .netstandard (which you can't create a library
that compatible .net framework and .net at same time)

At the same time, this repository provides CI tests project for automation tests, too (not configured yet).

More about LinePutScript library, please go to [LorisYounger/LinePutScript repository (Not translated)](https://github.com/LorisYounger/LinePutScript).

> BTW, This repository is only used for adding LinePutScript support to VPet.Avalonia.VPetSim (not uploaded yet.), it is not recommended to use it to your project due to the complexity usage and proprietary standard.