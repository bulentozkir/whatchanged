# ChangeTracker Yardımı

Geliştirme sürümünün çevrimdışı kılavuzu. Uygulama yapılandırmayı değiştirmeden gözlemler. Windows'u onarmaz, bilgisayarın güvenli olduğuna karar vermez.

## Dil

**Ayarlar > Dil** bölümünden seçin. Seçim hatırlanır; arayüz, açık yardım, tarihler ve metin raporları yeniden başlatma veya toplama olmadan güncellenir. Yirmi dil çevrimdışı dahildir. Arapça, Mısır Arapçası ve Urduca içerik sağdan sola; menü soldadır.

Program adları, sizin yazdığınız nokta adları, yollar, kimlikler ve özgün değerler çevrilmez. JSON/CSV sabit İngilizce alanları korur. Windows/UAC sistem dilini kullanır. Yayından önce ana dili konuşanların incelemesi gerekir.

## Ayarlar

Gezinme menüsünden **Ayarlar** bölümünü açın. Tercihler geçerli geçmiş klasörüne kaydedilir ve yeniden açılışta geri yüklenir.

- Dil anında uygulanır. Görünüm bölümünde açık/koyu tema, yazı tipi ve uygulama metni, etiketler, düğme arka planı ile düğme metni için bağımsız renkler vardır. Adlandırılmış renk örnekleri varsayılan, lacivert, orman yeşili, bordo ve mordur. Varsayılan, o öğenin tema rengini geri getirir; Windows yüksek karşıtlık renkleri önceliklidir.
- Otomatik görüntü alma varsayılan olarak kapalıdır. Aralıklar 15 dakika, 1 saat, 6 saat, günlük veya haftalıktır. Uygulama açıkken, tepside de normal yetkiyle çalışır; iptal edilebilir, yönetici izni istemez ve bilgisayarı uyandırmaz. Zamanı gelen işler dakikada bir kontrol edilir; yeniden açıldıktan sonra gecikmiş bir kontrol yapılabilir, kaçırılan aralıklar tekrar oynatılmaz.
- Saklama varsayılanı süresizdir. 30, 90, 180 veya 365 gün seçilirse yalnızca eski, adsız ve referans olmayan görüntüler silinir. Temizlik ilk zamanı geldiğinde, ardından uygulama açıkken günlük ve başarılı otomatik kontrollerden sonra çalışır; görüntü sıklığı kapalı olsa da işler. Adlandırılmış noktalar ve tüm kapsam/erişim referansları korunur.
- Oturum açılışında başlatma isteğe bağlı ve varsayılan olarak kapalıdır. Yeniden başlatmadan sonraki oturum açılışını da kapsar; oturum öncesi toplama yapmaz. Yalnızca uygulamanın kendi kullanıcı başlangıç kaydı değişir; hizmet veya açılış görevi kurulmaz, başka uygulama ya da ilkeye dokunulmaz. Başarısız kayıt önceki seçimi korur.
- Tepside çalıştırma isteğe bağlı ve varsayılan olarak kapalıdır. Küçült veya Kapat pencereyi gizler, kontroller sürer. Aç ya da uygulamayı tekrar başlatmak pencereyi getirir. Tepsideki Çıkış etkin toplamayı iptal eder ve uygulamayı kapatır. Tepsi kapalıysa Kapat da iptal edip çıkar.

Yeni profilde iki kapsam da seçilidir; kayıtlı tercihler korunur. Tüm kapsamların saklanan geçmişi sorgulanabilir, ancak karşılaştırmanın iki ucu aynı kapsam ve erişimde olmalıdır. Hiçbir tercih yönetici yetkisi vermez. Kaynak kullanımı profillemesi ve kurulu paket yaşam döngüsü doğrulaması henüz tamamlanmamıştır.

## Başlangıç

1. Normal açın; yönetici olarak değil.
2. Yeni profilde seçili gelen **Geçerli kullanıcı** ve **Bilgisayar geneli** kutularını inceleyin. Birini veya ikisini tutun; en az biri zorunludur. Onaylayın.
3. Kaynaklar bölümünü inceleyin. Ağ ve PATH isteğe bağlıdır. Seçim yapmak kontrol başlatmaz.
4. Bugün ve Şimdi kontrol et seçeneklerini kullanın.

İlk kullanılabilir gözlem kapsam ve erişim için referans olur. Bu bir envanterdir, geçmiş değişikliklerin yeniden oluşturulması değildir. Yeni profilde otomatik toplama kapalıdır; etkinleştirilen aralıklar yalnızca uygulama çalışırken uygulanır.

## Basit ve Gelişmiş

Basit özetleri ve metin raporlarını gösterir. Gelişmiş tüm kayıtlı alanları, özet sınırı olmadan önce/sonra değerlerini, üst verileri ve JSON/CSV çıktısını ekler. Tarih karşılaştırması, geçmiş ve kaynaklar iki modda da vardır.

Mod değiştirmek toplamaz, yetki yükseltmez, referansı değiştirmez ve ek ücret getirmez. Planlanan fiyat tek seferlik 0,99 ABD dolarıdır; önizlemede satın alma yoktur.

## Bağımsız kapsamlar

Kullanıcı kapsamı kendi uygulama kayıtları, Run/RunOnce, varsayılanlar, ses, proxy ve PATH'i içerir. Bilgisayar kapsamı ortak kayıtlar, hizmetler, görevler, güncellemeler, sürücüler, güvenlik duvarı, DNS/DHCP ve makine PATH'ini içerir. Başkalarının özel profilleri yüklenmez.

İki kutu seçilince parçalar ayrı okunur ve birleşik görüntüye kaydedilir. Yalnızca makine kısmı açık istekle yönetici olabilir. Kullanıcı kısmı özgün normal hesapta kalır. Kaydedilmiş kapsam ve kaynak tercihleri korunur.

## Tarihler ve gözlemler

Tarih seçicileri yalnızca saklanan görüntüleri sunar; her seçenek yerel tarih, milisaniyeli saat, UTC farkı, nokta ve kapsam/erişimi gösterir. Rastgele tarih yazılamaz; silinen kayıtlar listeden çıkar. İkinci uç kayıtlı görüntü veya Şimdi kontrol et ile alınan yeni gözlem olabilir. Referans düğmesi normal erişimli referansı seçer, değiştirmez. Yalnızca mevcut durum önceki seçimi temizler.

Görüntü olmayan gün yeniden oluşturulamaz; en yakın tarih sessizce seçilmez. Gözlemler ayrı, kronolojik, örtüşmeyen ve aynı kapsam/erişimde olmalıdır.

## İki kayıtlı görüntü

Önceki tarih ve görüntüyü seçin. Kayıtlı görüntü seçeneğiyle sonraki tarih ve görüntüyü seçip karşılaştırın. Toplayıcı çalışmaz, UAC istenmez, yeni kayıt oluşmaz. Referans değişmez. Aynı günün farklı saatleri karşılaştırılabilir.

## Görüntü ile bugün

Önceki görüntüyü ve Bugün'ü seçin. Şimdi kontrol et yeni gözlem oluşturur ve tam seçtiğiniz kayıtla karşılaştırır; başka referansa gizlice geçmez. Başarılı işlemden sonra panel kapanır, tekrar açılabilir.

Yönetici referansı otomatik yetki yükseltmez. Ayrı yönetici kontrolünü kullanın veya yalnızca mevcut durum alın. İptal etkin kontrolü durdurur; eski geçmiş korunur. Tepsi etkinse Kapat toplamayı sürdürür; tepside Çıkış iptal edip kapatır.

## Yönetici erişimi

Birçok makine ayarı normal izinle okunur; korumalı alanlar eksik kapsam olarak görünür. Yönetici eylemi açık tıklama, varsayılanı Hayır olan onay ve tek kontrol için Windows UAC izni gerektirir.

Ana pencere normal kalır. Geçici salt okunur yardımcı makineyi kontrol eder; hizmet kurmaz, kalıcı izin bırakmaz. Reddetmek geçmişi veya referansı değiştirmez. Parola paylaşmayın, cihaz ilkelerini kapatmayın. Açık UAC istemini uygulama kapatamaz; Windows'ta yanıtlayın.

## Değişiklik ayrıntıları

Eklendi, Kaldırıldı ve Değiştirildi gözlenen uçları açıklar; yapan kişiyi, kesin zamanı veya nedeni kanıtlamaz. Önemli/İncele önceliktir, zararlı yazılım kararı değildir. Olağan/beklenen ve etkisi bilinmeyen değişiklikler ayrı gruplardadır. Başlık etkin filtreyi sayar; Tümünü göster ilk üçün devamını açar.

Gelişmiş uzun değerleri, değişmeyen bağlamı, eklenen/silinen alanları, kayıt kimliğini ve kaynağı gösterir. Üst veriler görüntü kimlikleri, kapsam/erişim, UTC toplama/okuma zamanları, durum, sürümler ve sayılardır. Boş ile mevcut değil ayrıdır. Hiç saklanmayan komutlar geri oluşturulamaz; anahtarlar ve parmak izleri gizlidir. Kimlikler cihazı tanıtabilir; kopyalamadan inceleyin.

Beklenen işareti geri alınabilir ve yalnızca o olaya uygulanır. Ayarları aç, izinli Windows aracını açar; onarım yapmaz.

## Kapsam ve belirsizlik

Başarılı, uygulanan alt kümede tam demektir; tüm Windows değildir. Kısmi eksik kayıt veya sınır, Başarısız kullanılamaz okuma, Devre dışı/kapsam dışı okunmadı demektir. Eksik veriden silinme uydurulmaz.

Yeni tam okuma eski eksik veya biçim/anahtar uyumsuz gözlemle karşılaştırılamayabilir. Birleşik kapsamda tek eksik parça kategoriyi kısmi yapar. Kapsama ayrıntıları referans ve değişmeyen alanları tutar. Genel güvenlik ya da neden garantisi yoktur.

## Referans ve geçmiş

Görüntüler bölümünde görebilir, adlandırabilir (1–120 karakter), silebilir ve onayla referans yapabilirsiniz. Referansı silmeden başka referans seçin. Kullanıcı, makine, ikisi, erişim düzeyleri ve eski karma gözlemler ayrı referans tutar.

Sabit nokta sınırı yoktur. İsteğe bağlı saklama politikası eski adsız kayıtları temizler; adlandırılmış noktalar ve tüm referanslar korunur. Tamamen kullanılamaz toplama kaydedilmez. Geçmişi temizle onayla kayıtları ve işaretleri siler, tercihleri/anahtarı tutar; dışa aktarılanları ve Windows'u değiştirmez. Adli silme değildir.

## Kaynak sınırları

| Kaynak | Sınır |
| --- | --- |
| Uygulama ve başlangıç | Kaldırma kayıtları ve Run/RunOnce; Store, taşınabilir uygulama veya Başlangıç klasörü yok. Kayıt çalıştırma kanıtı değil. |
| Hizmet ve görev | Erişilebilir yapılandırma; çalıştırma, saklanan komut/XML veya sürekli sorgu yok. |
| Güncelleme ve sürücü | Yerel başarılı geçmiş, en çok 5.000 olay (aşılırsa kısmi), WMI bilgisi; kurulum, firmware veya geri alma yok. |
| Varsayılan ve ses | Desteklenen ilişkilendirmeler ve varsayılan cihazlar; ses kaydı veya değişiklik yok. |
| Koruma | Güvenlik duvarı profilleri, antivirüs değerlendirmesi değil. |
| Ağ ve PATH | İsteğe bağlı proxy veya DNS/DHCP ve kalıcı PATH; paket, parola, ağ taraması veya diğer değişkenler yok. |

Kaynak başına 25 saniye sınır vardır. Envanter ekranı kaynak başına 1.000 kayıt gösterir, alınan bütün kayıtlar saklanır. Salt okunur, kendi geçmişini ve istenen çıktıları yazabilir; izlenen ayarları değiştiremez.

## Raporlar

Rapor ekrandaki sonucu kullanır, henüz çalıştırılmamış tarih seçimini değil. İki modda metin önizleme/kopyalama/kaydetme; Gelişmiş'te JSON/CSV bulunur. Metin çevrilir, yapılandırılmış şema sabittir. Otomatik gönderim yoktur.

Anahtarlar, parmak izleri, başlatma değerleri, nokta adları ve ses uç kimlikleri rapordan çıkarılır. Profil yolları ve yaygın sır kalıpları maskelenir; CSV formülleri etkisizleştirilir. Tanımlayıcı adlar kalabilir. PDF/HTML, içe aktarma ve şifreli paketler yoktur. Geçmiş silinince çıktılar kalır.

## Gizlilik ve saklama

Normal konum `%LOCALAPPDATA%\PCChangeTracker`; Ayarlar gerçek konumu gösterir. SQLite şifreli değildir. Anahtar geçerli kullanıcı DPAPI korumasını kullanır; başka hesaba kopyalamak çözülmesini sağlamaz. Yeni sürümden önce güvenli yedek alın.

Geçmiş biçimi 2 eski kayıtları karma olarak korur ve eski okuyucuları engeller. Yardımcı yalnızca kategori ve geçici anahtar alır; geçmiş yolu veya serbest komut almaz. Normal arayüz kaydeder. MSIX veri yaşam döngüsü ayrıca test edilmelidir.

## Erişilebilirlik

Tab/Shift+Tab, oklar ve boşluk ile gezinilir. Kapsamlar bağımsız kutular, modlar radyodur. Tür ve öncelik yalnızca renk değil metindir. Görünür odak ve Windows yüksek karşıtlık renkleri desteklenir.

F1 yardım, Ctrl+F arama, Escape kapatır. Yardım yakınlaştırması %160'a kadar; dar tablolar etiketli satırlara dönüşür. Tam ekran okuyucu ve ana dil incelemesi hâlâ gereklidir.

Başlıklar ekran okuyucu gezinmesine açıktır. Ayrıntılar açılınca odak içeri taşınır; Tab içeride dolaşır, Escape kapatır. Renk ve yazı tipi Ayarlar'dadır; renkler temaya uyarlanır ve yüksek karşıtlık önceliklidir.

## Sorun giderme

Boş tarih: başka gözlem seçin. Reddedilen karşılaştırma: sıra, kapsam, erişim kontrol edin. Kısmi kaynak silinme demek değildir. Eski rapor: yeni seçimi önce çalıştırın. Açılmayan veritabanı: silmeden önce alan, izin, sürüm kontrol edin.

Destek için incelenmiş rapor ve uygulama/Windows sürümlerini paylaşın; parola, ham veritabanı, anahtar paylaşmayın. Yönetici iznini reddetmek normal kontrolü engellemez.

## Yayın durumu

11 sınırlı kategori, elle veya isteğe bağlı zamanlanmış kontroller ve saklama ayarları içeren önizlemedir. Sürekli olay izleme, bildirim ve tam zaman çizelgesi yoktur. Toplama kaynak kullanır; sıfır CPU sözü verilmez.

Yerel x64 MSI/MSIX imzasızdır. MSI kurulumu ayrıca izin ister; iki biçimi birlikte kurmayın. Gerçek UAC, farklı yönetici hesabı, Windows 10/ARM64, kurulum ve Store `allowElevation` onayı beklemektedir. Kod değişince eski paket otomatik yenilenmez. Logolar gerçek ekran görüntülerinin veya sertifikasyonun yerine geçmez.