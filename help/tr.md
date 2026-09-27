# ChangeTracker Yardımı

Geliştirme sürümünün çevrimdışı kılavuzu. Uygulama yapılandırmayı değiştirmeden gözlemler. Windows'u onarmaz, bilgisayarın güvenli olduğuna karar vermez.

## Dil

**Ayarlar > Dil** bölümünden seçin. Seçim hatırlanır; arayüz, açık yardım, tarihler ve metin raporları yeniden başlatma veya toplama olmadan güncellenir. Yirmi dil çevrimdışı dahildir. Arapça, Mısır Arapçası ve Urduca içerik sağdan sola; menü soldadır.

Program adları, sizin yazdığınız nokta adları, yollar, kimlikler ve özgün değerler çevrilmez. JSON/CSV sabit İngilizce alanları korur. Windows/UAC sistem dilini kullanır. Yayından önce ana dili konuşanların incelemesi gerekir.

## Ayarlar

Gezinme menüsünden **Ayarlar** bölümünü açın. Tercihler geçerli geçmiş klasörüne kaydedilir ve yeniden açılışta geri yüklenir.

- Dil anında uygulanır. Görünüm bölümünde açık/koyu tema, yazı tipi ve uygulama metni, etiketler, düğme arka planı ile düğme metni için bağımsız renkler vardır. Adlandırılmış renk örnekleri varsayılan, lacivert, orman yeşili, bordo ve mordur. Varsayılan, o öğenin tema rengini geri getirir; Windows yüksek karşıtlık renkleri önceliklidir. Kayıtlı bir seçim yoksa koyu tema varsayılandır. Düğmeler net bir hiyerarşi izler: ana kontrol mavi, referansı değiştirme kehribar, silme işlemleri kırmızıdır; diğer komutlar renkli bir simgeyle nötrdür (ör. Rapor, Kapsamı değiştir ve Yardım). Renkler az görme ve renk körlüğü düşünülerek seçilmiştir: etkileşimli öğeler mavidir, etiketlerin kendi rengi vardır ve her durum ayrıca bir sözcük ve simgeyle gösterilir. Açılır listeler, onay kutuları ve anahtarlar ok, onay işareti ve odak çerçevesi için vurgu rengini kullanır. Ayarlar bölümleri sütunlarda görünür, bu yüzden sayfa genellikle kaydırmadan tek ekrana sığar.
- Otomatik görüntü alma varsayılanı her 4 saatte birdir; Kapalı dahil kayıtlı seçimler korunur. Aralıklar 15 dakika, 1 saat, 4 saat, 6 saat, günlük veya haftalıktır; yalnızca elle kontrol için Kapalı seçin. Uygulama açıkken, tepside de normal yetkiyle çalışır ve iptal edilebilir. Kapsam onayından sonra ilk veya gecikmiş kontrol bir sonraki dakika denetiminde yapılabilir; sonraki kontroller seçilen aralıktadır. Yönetici izni istemez, bilgisayarı uyandırmaz ve kaçırılan aralıkları tekrarlamaz.
- Saklama varsayılanı 30 gündür; süresiz dahil kayıtlı seçimler korunur. 30, 90, 180 veya 365 gün ya da süresiz seçilebilir. Yalnızca eski, adsız ve referans olmayan görüntüler silinir. Temizlik ilk zamanı geldiğinde, ardından uygulama açıkken günlük ve başarılı otomatik kontrollerden sonra çalışır; görüntü sıklığı kapalı olsa da işler. Adlandırılmış noktalar ve tüm kapsam/erişim referansları korunur.
- Oturum açılışında başlatma isteğe bağlı ve varsayılan olarak kapalıdır. Yeniden başlatmadan sonraki oturum açılışını da kapsar; oturum öncesi toplama yapmaz. Yalnızca uygulamanın kendi kullanıcı başlangıç kaydı değişir; hizmet veya açılış görevi kurulmaz, başka uygulama ya da ilkeye dokunulmaz. Başarısız kayıt önceki seçimi korur.
- Küçült veya Kapat pencereyi her zaman tepsiye gizler, kontroller sürer. Aç, tepsi simgesine çift tıklama veya uygulamayı tekrar başlatmak pencereyi getirir. Tepsideki Çıkış etkin toplamayı iptal eder ve uygulamayı sonlandırır. Normal başlatma tam ekran boyutuna büyütülmüş pencereyle açılır; tepsiden dönüş son görünür durumu korur. İsteğe bağlı oturum açma başlangıcı, Windows yeniden başladıktan sonra da gizlidir; oturumdan önce çalışmaz ve hizmet değildir.

Basit görünüm, değişen açık alanları Önce/Sonra etiketleriyle daha büyük, seçilebilir ve salt okunur değerler olarak gösterir. Ayrıntılar kapanınca odak, varsa başlangıç düğmesine döner. Gelişmiş görünüm saklanan tarihleri kesin görüntü saatlerinden ayırır; kontrol noktası ve kapsam/erişim ayrı satırlardadır. Görünüm değişikliği seçili karşılaştırmayı korur.

Yeni profilde iki kapsam da seçilidir; kayıtlı tercihler korunur. Tüm kapsamların saklanan geçmişi sorgulanabilir, ancak karşılaştırmanın iki ucu aynı kapsam ve erişimde olmalıdır. Hiçbir tercih yönetici yetkisi vermez. Kaynak kullanımı profillemesi ve kurulu paket yaşam döngüsü doğrulaması henüz tamamlanmamıştır.

## Başlangıç

1. Normal açın; yönetici olarak değil.
2. Yeni profilde seçili gelen **Geçerli kullanıcı** ve **Bilgisayar geneli** kutularını inceleyin. Birini veya ikisini tutun; en az biri zorunludur. Onaylayın.
3. Kaynaklar bölümünü inceleyin. Ağ ve PATH dahil desteklenen tüm kontroller varsayılan olarak açıktır. Kayıtlı seçimler korunur; kaynakları kapatabilirsiniz. Seçim yapmak kontrol başlatmaz.
4. Bugün ve Şimdi kontrol et seçeneklerini kullanın.

İlk kullanılabilir gözlem kapsam ve erişim için referans olur. Bu bir envanterdir, geçmiş değişikliklerin yeniden oluşturulması değildir. Yeni profilde varsayılan aralık 4 saattir; kapsam onayından önce toplama başlamaz ve yalnızca uygulama çalışırken yapılır. Elle kontrol için sıklığı Kapalı yapın.

## Basit ve Gelişmiş

Basit özetleri ve metin raporlarını gösterir. Gelişmiş tüm kayıtlı alanları, özet sınırı olmadan önce/sonra değerlerini, üst verileri ve JSON/CSV çıktısını ekler. Tarih karşılaştırması, geçmiş ve kaynaklar iki modda da vardır.

Mod değiştirmek toplamaz, yetki yükseltmez, referansı değiştirmez ve ek ücret getirmez. Planlanan fiyat tek seferlik 0,99 ABD dolarıdır; önizlemede satın alma yoktur.

## Bağımsız kapsamlar

Kullanıcı kapsamı kendi uygulama kayıtları, Run/RunOnce, varsayılanlar, ses, proxy ve PATH'i içerir. Bilgisayar kapsamı ortak kayıtlar, hizmetler, görevler, güncellemeler, sürücüler, güvenlik duvarı, DNS/DHCP ve makine PATH'ini içerir. Başkalarının özel profilleri yüklenmez.

İki kutu seçilince parçalar ayrı okunur ve birleşik görüntüye kaydedilir. İki kapsam da kendi hesabınız altında standart erişim kullanır. Kaydedilmiş kapsam ve kaynak tercihleri korunur.

## Tarihler ve gözlemler

Tarih seçicileri yalnızca saklanan görüntüleri sunar; her seçenek yerel tarih, milisaniyeli saat, UTC farkı, nokta ve kapsam/erişimi gösterir. Rastgele tarih yazılamaz; silinen kayıtlar listeden çıkar. İkinci uç kayıtlı görüntü veya Şimdi kontrol et ile alınan yeni gözlem olabilir. Referans düğmesi normal erişimli referansı seçer, değiştirmez. Yalnızca mevcut durum önceki seçimi temizler. **Karşılaştırma** başlığı seçimin iki ucunu her zaman gösterir, bu yüzden daraltılmışken de okunabilir; kontrol veya karşılaştırmadan sonra ve başka bir sayfa açıldığında daralır.

Görüntü olmayan gün yeniden oluşturulamaz; en yakın tarih sessizce seçilmez. Gözlemler ayrı, kronolojik, örtüşmeyen ve aynı kapsam/erişimde olmalıdır.

## İki kayıtlı görüntü

Önceki tarih ve görüntüyü seçin. Kayıtlı görüntü seçeneğiyle sonraki tarih ve görüntüyü seçip karşılaştırın. Toplayıcı çalışmaz, UAC istenmez, yeni kayıt oluşmaz. Referans değişmez. Aynı günün farklı saatleri karşılaştırılabilir.

## Görüntü ile bugün

Önceki görüntüyü ve Bugün'ü seçin. Şimdi kontrol et yeni gözlem oluşturur ve tam seçtiğiniz kayıtla karşılaştırır; başka referansa gizlice geçmez. Başarılı işlemden sonra panel kapanır, tekrar açılabilir.

Önceki görüntü eski bir sürümde yönetici erişimiyle kaydedildiyse yeni bir kontrol için referans olamaz, çünkü kontroller her zaman standart erişim kullanır. Standart erişimli bir görüntü seçin veya **Yalnızca mevcut durum** kullanın. İki kayıtlı görüntü yine karşılaştırılabilir. İptal etkin kontrolü durdurur; eski geçmiş korunur. Kapat toplamayı tepside sürdürür; tepside Çıkış iptal edip sonlandırır.

## Yönetici erişimi

ChangeTracker hiçbir zaman yönetici erişimi istemez. Elle veya otomatik her kontrol, iki kapsamda da standart Windows izinlerinizle çalışır; bu yüzden Windows bir kontrol için asla UAC istemi göstermez. Yönetici modu, yükseltilmiş yardımcı veya arka plan hizmeti yoktur.

Makine genelindeki birçok ayar standart izinlerle okunabilir. Bir kaynak bu izinlerin okuyamadığı bir şey içerirse ChangeTracker izin yükseltmek yerine o kaynağı eksik bildirir. Eksik kaynaklar kapsam ayrıntılarında listelenir ve hiçbir zaman kaldırma önermez.

ChangeTracker'ı **Yönetici olarak çalıştır** ile başlatmak desteklenmez: uygulama bir ileti gösterip kapanır. Normal şekilde açın.

Eski bir sürümün yönetici erişimiyle kaydettiği görüntüler geçmişte kalır. Bunları görüntüleyebilir, birbirleriyle karşılaştırabilir ve raporlara ekleyebilirsiniz; ancak yeni bir kontrol için önceki görüntü olamazlar, çünkü yeni kontroller her zaman standart erişim kullanır. Standart erişimli bir görüntü seçin veya **Yalnızca mevcut durum** seçeneğini kullanın.

MSI kurulumu Windows'tan yönetici onayı gerektirir; bu onay yalnızca kurulum içindir, ChangeTracker kontrolleri için değildir. Microsoft Store paketi onsuz kurulur. Yönetici parolasını asla paylaşmayın.
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
| Ağ ve PATH | Varsayılan olarak açık: proxy veya DNS/DHCP ve kalıcı PATH; paket, parola, ağ taraması veya diğer değişkenler yok. |

Geçerli kayıtlı seçim yoksa desteklenen tüm kaynaklar varsayılan olarak açıktır. Daha önce kapattığınız kaynak güncellemeden sonra kapalı kalır; Kaynaklar bölümünden değiştirebilirsiniz. Açmak toplama başlatmaz veya yetki yükseltmez. Fark göstermek için iki uçta da kullanılabilir gözlem gerekir; eski görüntüler geriye dönük doldurulmaz.

Kaynak başına 25 saniye sınır vardır. Envanter ekranı kaynak başına 1.000 kayıt gösterir, alınan bütün kayıtlar saklanır. Salt okunur, kendi geçmişini ve istenen çıktıları yazabilir; izlenen ayarları değiştiremez.

## Raporlar

Rapor ekrandaki sonucu kullanır, henüz çalıştırılmamış tarih seçimini değil. İki modda metin önizleme/kopyalama/kaydetme; Gelişmiş'te JSON/CSV bulunur. Metin çevrilir, yapılandırılmış şema sabittir. Otomatik gönderim yoktur.

Anahtarlar, parmak izleri, başlatma değerleri, nokta adları ve ses uç kimlikleri rapordan çıkarılır. Profil yolları ve yaygın sır kalıpları maskelenir; CSV formülleri etkisizleştirilir. Tanımlayıcı adlar kalabilir. PDF/HTML, içe aktarma ve şifreli paketler yoktur. Geçmiş silinince çıktılar kalır.

## Gizlilik ve saklama

Normal konum `%LOCALAPPDATA%\PCChangeTracker`; Ayarlar gerçek konumu gösterir. SQLite şifreli değildir. Anahtar geçerli kullanıcı DPAPI korumasını kullanır; başka hesaba kopyalamak çözülmesini sağlamaz. Yeni sürümden önce güvenli yedek alın.

### Disk kullanımını yönetme

1. Sol çubukta Yardım'ın üstündeki görüntü depolama boyutuna bakın. Her sayfada görünür ve geçerli geçmişin tüm kapsamlarını içerir.
2. Bilgi için fareyi boyutun üzerine getirin. Tab ile etikete odaklanabilirsiniz; ekran okuyucular etiketin adını ve yardım metnini okuyabilir.
3. Ayarlar'daki otomatik kontrollerin sıklığını değiştirin. Daha uzun aralık daha az yeni görüntü üretir. Kapalı, otomatik toplamayı durdurur; geçmişi silmez ve saklama temizliğini durdurmaz.
4. Daha kısa saklama süresi uygun eski görüntüleri sonraki zamanı gelen temizlikte siler, seçim anında değil. Temizlik zamanı geldiğinde, sonra uygulama çalışırken günlük ve başarılı otomatik toplamalardan sonra yapılır. Tüm referanslar ve adlandırılmış kontrol noktaları korunur; bu kesin bir disk sınırı değildir.

Boyut, varsa `history.db`, `history.db-wal` ve `history.db-shm` dosyalarını toplar. Yalnızca görüntüleri değil tercihleri, veritabanı ek alanını ve yeniden kullanılabilir boş alanı içerir; Windows'un bloklara yuvarlanmış disk üzerindeki boyutu değildir. Dışa aktarımlar, uygulama kurulumu ve anahtar dosyası dahil değildir. B, KiB, MiB, GiB ve TiB, 1.024'ün katlarını ve seçili dilin sayı biçimini kullanır.

Değer toplama, silme veya temizlikten sonra geçmiş yenilendiğinde güncellenir; sürekli disk izlemesi değildir. Boyut alınamıyor, sıfır anlamına gelmez. Silme dosyayı küçültmeden yeniden kullanılabilir alan bırakabilir; boş geçmiş de yer kaplar. Otomatik veritabanı sıkıştırması yapılmaz. Uygulama çalışırken veritabanını veya geçici dosyalarını silmeyin.

Geçmiş biçimi 2 eski kayıtları karma olarak korur ve eski okuyucuları engeller. Yayınlanmamış bir derlemeyi kullanmadan önce önemli verileri yedekleyin. MSIX veri yönlendirme, sıfırlama ve kaldırma davranışları ayrıca test edilmelidir; yaşam döngüsünün MSI derlemesiyle aynı olduğunu varsaymayın.

## Erişilebilirlik

Ayarlar > Görünüm > Metin boyutu, 100%, 125%, 150% ve 200% seçeneklerini kaydeder; sayfaları, denetimleri, Yardım ve Rapor metnini büyütür. Alan çiftleri dar yerde alt alta gelir; sayfalar, yan çubuk ve iletişim kutuları kaydırılabilir. Yazı tipi seçimi yardım belgesine de uygulanır; Yardım'ın ayrı yakınlaştırması bu temel boyutun %160'ına kadar çıkabilir.

Yan gezinti seçili sayfayı bildirir ve ok tuşlarını destekler. Ctrl+1 değişiklikleri, Ctrl+2 görüntüleri, Ctrl+3 kaynakları, Ctrl+4 ayarları açar. F6 ve Shift+F6 gezinti, komut çubuğu ve sayfa başlığı arasında dolaşır; Tab sayfa denetimlerine devam eder. Yardım'da F6 arama, konular ve belge arasında dolaşır; Ctrl+F aramaya gider.

Kapsam seçimi ilk kutuya odaklanır; onay sonrasında Kapsamı değiştir'e döner. Modal paneller tüm yan çubuğu ve sayfa kısayollarını devre dışı bırakır; Tab panel içinde kalır. Escape ayrıntıları kapatır ve varsa başlangıç eylemine odak döner. Yardım aramayla, Rapor salt okunur önizlemeyle başlar. Görüntü satırları, konular, gruplar ve alanlar okunabilir adlar taşır; değerler alanı ve Önce/Sonra tarafını, kaynaklar durumu ve kapsamı bildirir. Ana denetimlerin etkileşim yüksekliği en az 36 cihazdan bağımsız birimdir; bu, WCAG 2.2 AA'nın 24 piksellik en küçük hedef boyutunun üzerindedir. Devre dışı düğmeler kesikli çerçeveyle gösterilir.

Bu özellikler evrensel uyumluluk sertifikası değildir. Ekran okuyucular, karşıtlık temaları, Windows ölçekleri ve engelli kullanıcılarla elle değerlendirme gerekir. Gerçek klavye testleri kilidi açık ve müdahale edilmeyen oturum ister.

Tab/Shift+Tab, oklar ve boşluk ile gezinilir. Kapsamlar bağımsız kutular, modlar radyodur. Tür ve öncelik yalnızca renk değil metindir. Görünür odak ve Windows yüksek karşıtlık renkleri desteklenir.

F1 yardım, Ctrl+F arama, Escape kapatır. Yardım yakınlaştırması %160'a kadar; dar tablolar etiketli satırlara dönüşür. Tam ekran okuyucu ve ana dil incelemesi hâlâ gereklidir.

Başlıklar ekran okuyucu gezinmesine açıktır. Ayrıntılar açılınca odak içeri taşınır; Tab içeride dolaşır, Escape kapatır. Renk ve yazı tipi Ayarlar'dadır; renkler temaya uyarlanır ve yüksek karşıtlık önceliklidir.

## Sorun giderme

Boş tarih: başka gözlem seçin. Reddedilen karşılaştırma: sıra, kapsam, erişim kontrol edin. Kısmi kaynak silinme demek değildir. Eski rapor: yeni seçimi önce çalıştırın. Açılmayan veritabanı: silmeden önce alan, izin, sürüm kontrol edin.

Destek için incelenmiş rapor ve uygulama/Windows sürümlerini paylaşın; parola, ham veritabanı, anahtar paylaşmayın. Bazı makine geneli ayarlar yönetici hakları gerektirir; ChangeTracker yükseltme istemek yerine bunları eksik bildirir ve diğer kaynakları karşılaştırmaya devam eder.

## Yayın durumu

11 sınırlı kategori, elle veya isteğe bağlı zamanlanmış kontroller ve saklama ayarları içeren önizlemedir. Sürekli olay izleme, bildirim ve tam zaman çizelgesi yoktur. Toplama kaynak kullanır; sıfır CPU sözü verilmez.

Yerel sürüm x64 ve ARM64 MSI yükleyicileri ile x64 MSIX paketini içerir; hepsi imzasızdır. MSI kurulumu yönetici onayı gerektirir, ancak kurulu uygulama her zaman standart izinlerle çalışır. İmzalama, Store sertifikasyonu, Windows 10/ARM64 yeterliliği ve kurulum/yükseltme/kaldırma yeterliliği beklemektedir. Kod değişince eski paket otomatik yenilenmez. Logolar gerçek ekran görüntülerinin veya sertifikasyonun yerine geçmez.