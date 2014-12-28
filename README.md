# BlowfishManaged
=================

A C# managed-memory implementation of the Blowfish cipher in .NET's Security interface.
Designed with performance and code aesthetics in mind. Benchmarks are at the bottom of this page.
Like the Blowfish cipher, this code is public domain.

## Quick Example

Blowfish works on 64-bit blocks. You can pass a 64-bit integer to be encrypted directly.

```C#
BlowfishManaged.BlowfishManaged Blowfish = new BlowfishManaged.BlowfishManaged();
Blowfish.Key = Encoding.UTF8.GetBytes("secret key");

UInt64 Plaintext = 0xABCDEF1234C0FFEE;
UInt64 Encrypted = Blowfish.EncryptSingleBlock(Plaintext);
Console.WriteLine(String.Format("0x{0:X}", Encrypted)); // 0x392BB6800FB73753

UInt64 Decrypted = Blowfish.DecryptSingleBlock(Encrypted);
Console.WriteLine(String.Format("0x{0:X}", Decrypted)); // 0xABCDEF1234C0FFEE
```

## Encrypt a stream in CBC mode

BlowfishManaged does not implement CBC mode internally, due to .NET limitations.
Microsoft's AesManaged implementation gets around this by making unsafe calls to memory directly, but it is not difficult to set up a CBC stream yourself.

```C#
byte[] plaintext = new byte[512];
byte[] encrypted;
byte[] decrypted = new byte[512];

SymmetricAlgorithm blowfish = new BlowfishManaged.BlowfishManaged();
blowfish.Key = Encoding.UTF8.GetBytes("secret key");
blowfish.GenerateIV();
```

For an encryption stream, create a *Write*-able *CryptoStream* with a new memory stream and a call to **BlowfishManaged.CreateEncryptor()**.
All bytes written into the crypto stream will be encrypted and accessible via the memory stream.

```C#
using (MemoryStream memoryStream = new MemoryStream())
{
    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, blowfish.CreateEncryptor(), CryptoStreamMode.Write))
        cryptoStream.Write(plaintext, 0, plaintext.Length);
    encrypted = memoryStream.GetBuffer();
}
```

For an encryption stream, create a *Read*-able *CryptoStream* with a new memory stream and a call to **BlowfishManaged.CreateDecryptor()**.
If you instantiate the memory stream with a byte array, it will automatically funnel all bytes through the crypto stream. Reading from the crypto stream will return decrypted data.

```C#
using (MemoryStream memoryStream = new MemoryStream(encrypted))
using (CryptoStream cryptoStream = new CryptoStream(memoryStream, blowfish.CreateDecryptor(), CryptoStreamMode.Read))
{
    int bytesRead = cryptoStream.Read(decrypted, 0, decrypted.Length);
}
```

The decrypted and plaintext arrays are identical at this point.

## Performance

The most expensive part of encryption with Blowfish is the key-dependent s-box and round key generation steps. A total of 521 encryption iterations are needed just to set up all of the round keys. This discourages brute-force key guessing.

I tested my implementation against the C# Blowfish implementation provided on the inventor's website at
* https://www.schneier.com/blowfish-download.html

Measured on an Intel i7 processor,

 | Generate 100,000 Key Schedules  | Encrypt 1GB Data | Decrypt 1GB Data
------------- | ------------- | ------------- | -------------
BlowfishManaged  | 5.68 μs / K.S. | 141.469 ms / MB | 146.259 ms / MB
BlowfishCS  | 19.48μs / K.S. | 266.387 ms / MB | 276.892 ms / MB

On average, my BlowfishManaged library provides substantial gains over the open-source implementation.

