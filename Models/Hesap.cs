using System;
using System.Collections.Generic;

namespace Odeme_Projesi.Models;

public class Hesap{
    public int Id{get;set;}
    public string HesapNo{ get ; set;} //unique
    public decimal  KumuleAlacak{ get ; private set;}
    public decimal KumuleBorc{ get ; private set;}
    public decimal Bakiye {get;private set;}
    public int MusteriId { get; set; } 
    public Musteri Musteri { get; set; }  
    
    

    public Hesap() { }
   
    public Hesap(string hesapNo, int musteriId, decimal kumuleAlacak, decimal kumuleBorc)
{
    HesapNo = hesapNo;
    MusteriId = musteriId;
    KumuleAlacak = kumuleAlacak;
    KumuleBorc = kumuleBorc;
}

    public decimal getBakiye() => this.KumuleAlacak-this.KumuleBorc;
    
    public void KumuleAlacakArttir(decimal miktar)
    {
        if (miktar <= 0)
            throw new ArgumentException("Miktar sıfırdan büyük olmalı!");

        this.KumuleAlacak += miktar;
    }

    public bool KumuleBorcArttir(decimal miktar)
    {
        if (miktar <= 0)
            throw new ArgumentException("Miktar sıfırdan büyük olmalı!");

        if (Bakiye < miktar)
            return false;  // Yetersiz bakiye



        this.KumuleBorc += miktar;

        return true;
    }
    
}