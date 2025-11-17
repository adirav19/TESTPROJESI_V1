# 🏭 UAK (Üretim Akış Kaydı - ProductionFlow) Modülü Ekleme Kılavuzu

## 📋 Yapılan Değişiklikler

### 1. **Yeni Controller Eklendi**
📁 `Controllers/UakController.cs`
- BaseApiController'dan türetilmiş
- ProductionFlow endpoint'ine bağlı
- GET, POST, PUT, DELETE işlemleri destekli
- İş Emri No ile arama özelliği

### 2. **Yeni View Sayfası Eklendi**
📁 `Views/Uak/Index.cshtml`
- Tam özellikli UAK yönetim sayfası
- Oluşturma, güncelleme, silme formları
- İş emri bazlı sorgulama
- Dinamik tablo listesi
- Responsive tasarım

### 3. **Menü Güncellemesi**
📁 `Views/Shared/_Layout.cshtml`
- UAK modülü menüye eklendi
- İkon: `bi-gear-wide-connected`
- Menü sırası: Stoklar ile Faturalar arasında

## 🚀 Kurulum Adımları

### Adım 1: Dosyaları Projenize Ekleyin

```bash
# Controller'ı kopyalayın
/Controllers/UakController.cs  →  TESTPROJESI/Controllers/

# View dosyalarını kopyalayın
/Views/Uak/Index.cshtml  →  TESTPROJESI/Views/Uak/
/Views/Shared/_Layout.cshtml  →  TESTPROJESI/Views/Shared/
```

### Adım 2: Layout.cshtml'i Güncelleyin (Manuel)

Eğer mevcut _Layout.cshtml dosyanızı korumak istiyorsanız, sadece menü kısmını güncelleyin:

```csharp
var menuItems = new[]
{
    new { Controller = "Home", Action = "Index", Icon = "bi-house", Text = "Ana Sayfa" },
    new { Controller = "TestApi", Action = "Index", Icon = "bi-people", Text = "Cari Yönetimi" },
    new { Controller = "Stok", Action = "Index", Icon = "bi-box", Text = "Stoklar" },
    // ⬇️ YENİ SATIR
    new { Controller = "Uak", Action = "Index", Icon = "bi-gear-wide-connected", Text = "Üretim Akışı (UAK)" },
    // ⬆️ YENİ SATIR
    new { Controller = "Fatura", Action = "Index", Icon = "bi-receipt", Text = "Faturalar" },
    new { Controller = "Rapor", Action = "Index", Icon = "bi-graph-up", Text = "Raporlar" },
    new { Controller = "Auth", Action = "Index", Icon = "bi-key", Text = "Token Yönetimi" },
    new { Controller = "Ayar", Action = "Index", Icon = "bi-gear", Text = "Ayarlar" }
};
```

### Adım 3: Projeyi Derleyin ve Çalıştırın

```bash
cd TESTPROJESI
dotnet build
dotnet run
```

### Adım 4: Tarayıcıda Test Edin

```
https://localhost:7123/Uak/Index
```

## 📝 API Endpoint'leri

### GET İşlemleri
- `GET /Uak/Index` - Ana sayfa
- `GET /Uak/ListUaklar` - Tüm UAK kayıtlarını listele
- `GET /Uak/GetByIsEmriNo?isEmriNo=XXX` - İş emri ile sorgula

### POST İşlemleri
- `POST /Uak/CreateUak` - Yeni UAK kaydı oluştur
- `POST /Uak/UpdateUak` - Mevcut kaydı güncelle
- `POST /Uak/DeleteUak` - Kayıt sil

## 🎨 Özellikler

### ✅ Oluşturma (Create)
- İş Emri No, Stok Kodu, Operasyon bilgileri
- Tarih/saat seçimi (datetime-local)
- Vardiya seçimi (1-5)
- Süre ve süre tipi
- Üretim ve fire miktarları
- USK Depo kodu

### ✏️ Güncelleme (Update)
- Tüm alanlar düzenlenebilir
- IncKeyNo otomatik readonly
- Tablodan seçim ile otomatik form doldurma

### 🗑️ Silme (Delete)
- IncKeyNo ile silme
- Onay mesajı
- Güvenli silme işlemi

### 🔍 Sorgulama
- İş Emri No ile arama
- Sonuçlar tabloda vurgulanır
- Filtrelenmiş liste görünümü

### 📋 Listeleme
- Sayfalandırılmış tablo
- 50 kayıt limit
- IncKeyNo'ya göre sıralama
- Responsive tasarım
- İşlendi durumu badge'i
- Satır seçimi ile form doldurma

## 🔧 API Yapılandırması

Controller, aşağıdaki API endpoint'lerini kullanır:

```csharp
Base URL: api/v2/ProductionFlow

GET    api/v2/ProductionFlow?limit=50&sort=IncKeyNo ASC
GET    api/v2/ProductionFlow?q=ISEMRINO='XXX'
POST   api/v2/ProductionFlow
PUT    api/v2/ProductionFlow/{id}
DELETE api/v2/ProductionFlow/{id}
```

## 📊 Veri Modeli

### Request Örneği (POST)
```json
{
  "IsEmriNo": "000000000000001",
  "CONFSIRANO": "00000001",
  "StokKodu": "STOK001",
  "OpKodu": "OP.001",
  "OPSIRANO": "0001",
  "IstasyonKodu": "IST001",
  "SIMULTANEOPR": 1.0,
  "BASLANGICTARIH": "2025-11-17 10:00:00",
  "BASLANGICVARDIYA": 1,
  "SURE": 60.0,
  "SURETIPI": 0,
  "BITISTARIHSAAT": "2025-11-17 11:00:00",
  "AKTIVITEKODU": "01",
  "ISLENDI": false,
  "URETILENMIKTAR": 100.0,
  "FIREMIKTAR": 0.0,
  "RevNo": "00000000",
  "USKDEPOKODU": 10,
  "BASLADI_BITMEDI": false,
  "OLCUBRMIKTAR": 0,
  "OLCUBRFIRE": 0
}
```

### Response Örneği (GET)
```json
{
  "Offset": 0,
  "TotalCount": 14,
  "Limit": 10,
  "IsSuccessful": true,
  "Data": [
    {
      "IsEmriNo": "000000000000007",
      "CONFSIRANO": "00000001",
      "IncKeyNo": 6,
      "StokKodu": "YM31",
      "OpKodu": "OP.LZR",
      "OPSIRANO": "0001",
      "IstasyonKodu": "LAZER",
      "BASLANGICTARIH": "2023-11-07 10:24:49",
      "BASLANGICVARDIYA": 1,
      "SURE": 60.0,
      "ISLENDI": false,
      "URETILENMIKTAR": 100.0,
      "FIREMIKTAR": 0.0
    }
  ]
}
```

## 🎯 Form Alanları

| Alan | Tip | Zorunlu | Açıklama |
|------|-----|---------|----------|
| İş Emri No | Text | ✅ | İş emri numarası |
| Conf Sıra No | Text | ✅ | Konfigürasyon sıra no |
| Stok Kodu | Text | ✅ | Stok kodu |
| Operasyon Kodu | Text | ✅ | Operasyon kodu |
| Op Sıra No | Text | ✅ | Operasyon sıra no |
| İstasyon Kodu | Text | ✅ | İstasyon kodu |
| Başlangıç Tarihi | DateTime | ✅ | İşlem başlangıç tarihi |
| Başlangıç Vardiyası | Select | ✅ | Vardiya (1-5) |
| Süre | Number | ✅ | Dakika cinsinden süre |
| Süre Tipi | Select | ✅ | 0:Standart, 1:Hazırlık, 2:Operasyon |
| Bitiş Tarih Saat | DateTime | ✅ | İşlem bitiş tarihi |
| Aktivite Kodu | Text | ✅ | Aktivite kodu |
| Üretilen Miktar | Number | ✅ | Üretilen miktar |
| Fire Miktarı | Number | ❌ | Fire miktarı |
| Revizyon No | Text | ❌ | Revizyon numarası |
| USK Depo Kodu | Number | ✅ | USK depo kodu |

## 🔐 Token Yönetimi

UAK modülü, mevcut TokenManager servisini kullanır:
- Otomatik token alma
- Token cache yönetimi
- Hata yönetimi
- Loglama

## 🐛 Hata Yönetimi

- Try-catch blokları ile güvenli hata yakalama
- Kullanıcı dostu hata mesajları
- Console loglama
- ViewBag ile hata gösterimi

## 📱 Responsive Tasarım

- Mobile uyumlu
- Bootstrap 5.3
- Grid sistemi
- Card yapısı
- Responsive tablo

## 🎨 UI/UX Özellikleri

- ✅ Modern card tasarımı
- ✅ Renkli durum badge'leri
- ✅ Hover efektleri
- ✅ Loading spinner'ları
- ✅ Başarı/hata mesajları
- ✅ Smooth scroll animasyonları
- ✅ Görsel feedback (yeşil flash efekti)

## 🔄 Veri Akışı

```
Kullanıcı → Form → Controller → BaseApiService → API
                                       ↓
                                  TokenManager
                                       ↓
                                    NetOpenX
```

## 📚 Bağımlılıklar

- ✅ BaseApiController (mevcut)
- ✅ TokenManager (mevcut)
- ✅ BaseApiService (mevcut)
- ✅ Bootstrap 5.3
- ✅ Bootstrap Icons

## 🧪 Test Senaryoları

### 1. Yeni Kayıt Oluşturma
1. Formu doldurun
2. "Kayıt Oluştur" butonuna tıklayın
3. Başarı mesajını kontrol edin
4. Tabloda yeni kaydı görün

### 2. Kayıt Güncelleme
1. Tablodan bir satır seçin
2. Güncelleme formuna otomatik dolsun
3. Değişiklikleri yapın
4. "Güncelle" butonuna tıklayın

### 3. Kayıt Silme
1. Tablodan IncKeyNo'yu not edin
2. Silme formuna girin
3. Onaylayın ve silin

### 4. İş Emri Sorgulama
1. İş Emri No girin
2. "Sorgula" butonuna tıklayın
3. Sonuçları görün

## 💡 İpuçları

1. **Tarih Formatı**: API datetime formatını destekler: `"2025-11-17 10:00:00"`
2. **Vardiya**: 1-5 arası değer alır
3. **Süre Tipi**: 0=Standart, 1=Hazırlık, 2=Operasyon
4. **IncKeyNo**: Otomatik artan, güncelleme/silme için gerekli
5. **Token**: Otomatik yönetilir, manuel müdahale gereksiz

## 🔍 Sorun Giderme

### Problem: 401 Unauthorized
**Çözüm**: Token'ı kontrol edin, AuthController'dan yeni token alın

### Problem: Veri gelmiyor
**Çözüm**: API endpoint'ini ve limit parametresini kontrol edin

### Problem: Tarih formatı hatası
**Çözüm**: DateTime formatını kontrol edin: `YYYY-MM-DDTHH:MM`

### Problem: Form gönderilmiyor
**Çözüm**: Required alanları doldurun, console'da hataları kontrol edin

## 📞 Destek

Herhangi bir sorun için:
- Console loglarını kontrol edin
- Network tab'ı inceleyin
- API response'larını kontrol edin
- Token durumunu kontrol edin

## ✅ Tamamlandı

- [x] UakController oluşturuldu
- [x] Views/Uak/Index.cshtml oluşturuldu
- [x] Menü güncellendi
- [x] GET endpoint'i eklendi
- [x] POST endpoint'i eklendi
- [x] PUT endpoint'i eklendi
- [x] DELETE endpoint'i eklendi
- [x] İş emri sorgulama eklendi
- [x] Responsive tasarım
- [x] Form validasyonları
- [x] Hata yönetimi
- [x] Loglama

## 🎉 Sonuç

UAK modülü başarıyla projenize entegre edilmiştir. Mevcut yapıya uygun, temiz ve sürdürülebilir bir kod yapısı kullanılmıştır.

**Başarılı bir kullanım dileriz! 🚀**
