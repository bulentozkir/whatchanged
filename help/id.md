# Bantuan ChangeTracker

Panduan luring untuk pratinjau. Aplikasi membaca konfigurasi, tidak memperbaiki Windows, mengubah pengaturan, atau menjamin PC aman.

## Bahasa

Pilih **Pengaturan > Bahasa**. Pilihan disimpan dan memperbarui antarmuka, bantuan terbuka, tanggal, serta laporan teks tanpa mulai ulang atau pengumpulan. Dua puluh bahasa dibundel offline. Arab, Arab Mesir, dan Urdu memakai isi kanan-ke-kiri, menu tetap di kiri.

Nama aplikasi, label buatan pengguna, jalur, ID, dan nilai asli tidak diterjemahkan. JSON/CSV mempertahankan skema Inggris. Dialog Windows/UAC mengikuti bahasa Windows. Peninjauan penutur asli masih diperlukan sebelum rilis.

## Pengaturan

Buka Pengaturan dari menu. Preferensi disimpan untuk folder riwayat saat ini dan dipulihkan saat aplikasi dibuka kembali.

- Tampilan menyediakan tema terang/gelap, font, serta warna teks aplikasi, label, latar tombol dan teks tombol secara terpisah. Sampel bernama: Default, Biru tua, Hijau hutan, Merah marun dan Ungu. Default mengembalikan warna tema; kontras tinggi Windows selalu diutamakan dan tindakan utama tetap memakai teks kontras. Jika belum ada pilihan tersimpan, tema gelap menjadi default. Tombol mengikuti hierarki yang jelas: pemeriksaan utama berwarna biru, penggantian acuan berwarna kuning tua, penghapusan berwarna merah, dan semua perintah lain netral dengan ikon berwarna (misalnya Laporan, Ubah cakupan, dan Bantuan). Warna dirancang untuk penglihatan rendah dan buta warna: elemen interaktif berwarna biru, label memiliki warna sendiri, dan setiap status juga memiliki kata dan simbol. Daftar tarik-turun, kotak centang, dan sakelar memakai warna aksen untuk panah, tanda centang, dan garis fokus. Bagian Pengaturan tampil dalam kolom sehingga halaman biasanya muat dalam satu layar tanpa menggulir.
- Snapshot otomatis default setiap 4 jam; pilihan tersimpan, termasuk Nonaktif, dipertahankan. Pilih setiap 15 menit, 1 jam, 4 jam, 6 jam, hari atau minggu, atau Nonaktif untuk pemeriksaan manual saja. Pemeriksaan hanya berjalan selama aplikasi terbuka, termasuk di baki sistem, dengan akses biasa dan dapat dibatalkan. Setelah cakupan dikonfirmasi, pemeriksaan pertama atau terlambat dapat berjalan pada pengecekan menit berikutnya; selanjutnya mengikuti interval. Tidak meminta administrator, membangunkan PC, atau mengulang seluruh interval yang terlewat.
- Retensi default 30 hari; pilihan tersimpan, termasuk selamanya, dipertahankan. Pilih 30, 90, 180 atau 365 hari, atau selamanya. Hanya snapshot lama tanpa nama yang bukan acuan dihapus. Pembersihan berjalan saat pertama kali jatuh tempo, lalu setiap hari selama aplikasi terbuka dan setelah pemeriksaan otomatis berhasil, meskipun snapshot otomatis nonaktif. Checkpoint bernama dan semua acuan dilindungi.
- Mulai saat masuk Windows bersifat opsional dan awalnya nonaktif. Ini mencakup masuk setelah restart, bukan pengumpulan sebelum masuk. Hanya entri startup milik aplikasi untuk pengguna ini yang diubah; tidak memasang layanan atau tugas boot, dan tidak mengubah aplikasi lain atau kebijakan. Kegagalan mempertahankan pilihan sebelumnya.
- Minimalkan atau Tutup selalu menyembunyikan jendela ke baki sementara pemeriksaan berlanjut. Buka, klik dua kali ikon, atau menjalankan aplikasi lagi memulihkannya. Keluar dari baki membatalkan pekerjaan aktif dan mengakhiri aplikasi. Peluncuran biasa membuka jendela maksimal; pemulihan dari baki mempertahankan keadaan terakhir. Mulai saat masuk bersifat opsional dan tersembunyi, juga setelah Windows dimulai ulang; tidak berjalan sebelum masuk atau sebagai layanan.

Sederhana menampilkan bidang publik yang berubah dengan label Sebelum/Sesudah dan nilai lebih besar, dapat dipilih, serta hanya-baca. Menutup detail mengembalikan fokus ke tombol asal jika tersedia. Lanjutan memisahkan tanggal tersimpan dari waktu pengambilan tepat, dengan titik pemeriksaan dan cakupan/akses pada baris terpisah. Mengganti mode mempertahankan pasangan yang dipilih.

Profil baru memilih kedua cakupan; pilihan tersimpan tetap dihormati. Semua riwayat yang dipertahankan dapat dilihat dari cakupan mana pun, tetapi kedua ujung perbandingan harus memiliki cakupan dan akses sama. Preferensi tidak memberikan administrator. Profil sumber daya dan pengujian paket terpasang belum selesai.

## Mulai

1. Buka biasa, bukan sebagai administrator.
2. Pengguna saat ini dan Seluruh komputer sama-sama dicentang untuk profil baru. Pilih satu atau keduanya, tidak boleh kosong, lalu konfirmasi.
3. Tinjau Sumber. Semua pemeriksaan yang didukung, termasuk jaringan dan PATH, aktif secara bawaan. Pilihan tersimpan dipertahankan; sumber dapat dinonaktifkan. Mengubah pilihan tidak memulai pemeriksaan.
4. Pilih Hari ini dan Periksa sekarang.

Pembacaan pertama yang berguna menjadi acuan untuk cakupan dan akses itu. Ini inventaris, bukan rekonstruksi masa lalu. Profil baru memakai interval 4 jam, hanya setelah cakupan dikonfirmasi dan saat aplikasi berjalan. Pilih Nonaktif untuk pemeriksaan manual saja.

## Sederhana dan Lanjutan

Sederhana memberi ringkasan dan teks. Lanjutan menampilkan semua bidang, nilai sebelum/sesudah lengkap tanpa batas ringkasan, metadata, serta JSON/CSV. Kedua mode memiliki perbandingan tanggal, riwayat, dan sumber.

Pergantian mode tidak mengumpulkan, meningkatkan izin, memindahkan acuan, atau menagih biaya tambahan. Harga rencana US$0.99 sekali bayar mencakup keduanya. Pratinjau belum memiliki pembelian.

## Cakupan independen

Pengguna membaca registrasi aplikasi sendiri, Run/RunOnce, asosiasi, audio, proksi, PATH. Komputer membaca registrasi bersama, layanan, tugas, pembaruan, driver, firewall, DNS/DHCP, PATH sistem. Profil pribadi orang lain tidak dimuat.

Jika keduanya dipilih, kedua bagian dibaca terpisah dengan akses biasa pada akun Anda sendiri lalu menjadi satu snapshot gabungan. Tidak ada cakupan yang diberi hak administrator. Pilihan tersimpan dihormati.

## Tanggal dan pengamatan

Pemilih hanya menawarkan snapshot yang masih disimpan, dengan tanggal lokal, waktu beserta milidetik, selisih UTC, checkpoint dan cakupan/akses. Tanggal bebas tidak dapat diketik; snapshot terhapus hilang dari pilihan. Ujung kedua bisa snapshot tersimpan atau pemeriksaan baru Hari ini. Acuan memilih acuan akses biasa tanpa menggantinya. Hanya keadaan sekarang menghapus referensi sebelumnya. Judul **Perbandingan** selalu menyebutkan kedua ujung pilihan sehingga tetap terbaca saat diciutkan; judul ini menciut setelah pemeriksaan atau perbandingan dan saat Anda membuka halaman lain.

Hari tanpa snapshot tidak dapat direkonstruksi dan tidak otomatis diganti hari terdekat. Kedua pengamatan harus berbeda, berurutan, tidak tumpang tindih, serta memiliki cakupan dan akses sama.

## Dua snapshot tersimpan

Pilih tanggal dan snapshot sebelumnya, lalu Snapshot tersimpan beserta tanggal dan snapshot berikutnya. Bandingkan snapshot tidak menjalankan pengumpul, UAC, atau membuat rekaman baru. Acuan tidak berubah. Waktu berbeda pada hari yang sama dapat dibandingkan.

## Snapshot dengan hari ini

Pilih referensi dan Hari ini. Periksa sekarang membuat pengamatan baru dan membandingkan pilihan Anda, bukan acuan lain diam-diam. Panel menutup setelah berhasil dan dapat dibuka lagi.

Snapshot yang disimpan dengan akses administrator oleh versi lama tidak dapat menjadi referensi untuk pemeriksaan baru karena pemeriksaan selalu memakai akses biasa. Pilih snapshot akses biasa atau Hanya keadaan sekarang. Dua snapshot tersimpan tetap dapat dibandingkan. Batal menghentikan pengumpulan dan mempertahankan riwayat. Tutup membiarkan pengumpulan berlanjut di baki; Keluar dari baki membatalkan dan mengakhiri aplikasi.

## Administrator

ChangeTracker tidak pernah meminta akses administrator. Setiap pemeriksaan, manual maupun otomatis, berjalan dengan izin Windows biasa di kedua cakupan, jadi Windows tidak menampilkan prompt UAC untuk pemeriksaan. Tidak ada mode administrator, pembantu yang ditinggikan, atau layanan latar belakang.

Banyak pengaturan seluruh komputer dapat dibaca dengan akses biasa. Jika suatu sumber berisi hal yang tidak dapat dibaca oleh izin biasa, sumber itu dilaporkan tidak lengkap (tercantum di rincian cakupan, tidak pernah menyarankan penghapusan), bukan menaikkan izin. Menjalankan ChangeTracker dengan “Jalankan sebagai administrator” tidak didukung: aplikasi menampilkan pesan lalu menutup; buka secara normal.

Snapshot yang disimpan dengan akses administrator oleh versi lama tetap ada di riwayat: dapat dilihat, dibandingkan satu sama lain, dan disertakan dalam laporan, tetapi tidak dapat menjadi snapshot sebelumnya untuk pemeriksaan baru; pilih snapshot akses biasa atau “Hanya keadaan sekarang”. Instalasi MSI memerlukan persetujuan administrator dari Windows (hanya instalasi, bukan pemeriksaan); paket Microsoft Store memasang tanpa itu. Jangan pernah membagikan sandi administrator.

## Rincian perubahan

Ditambahkan, Dihapus, Diubah menerangkan perbedaan dua pengamatan, bukan pelaku, waktu persis, atau sebab. Penting/Tinjau adalah prioritas, bukan putusan malware. Rutin/diharapkan dan dampak belum dinilai terpisah. Judul mengikuti filter; Lihat semua menampilkan lebih dari tiga awal.

Lanjutan memuat nilai panjang, konteks tetap, bidang baru/hilang, identitas dan sumber. Metadata mencakup ID, cakupan/akses, waktu UTC snapshot/pembacaan, versi, status, jumlah. Kosong berbeda dengan tidak ada. Perintah yang tak pernah disimpan tidak dapat dikembalikan; kunci dan sidik jari tetap tersembunyi. ID dapat mengenali perangkat: periksa sebelum menyalin.

Tanda diharapkan hanya untuk kejadian ini dan dapat dibatalkan. Buka pengaturan membuka alat Windows yang diizinkan tanpa memperbaiki apa pun.

## Cakupan dan ketidakpastian

Berhasil berarti lengkap dalam bagian yang diimplementasikan, bukan seluruh Windows. Sebagian berarti entri hilang atau batas; Gagal pembacaan tak berguna; Nonaktif/di luar cakupan berarti tidak dibaca. Data tak lengkap tidak berarti penghapusan.

Pengamatan saat ini yang lengkap dapat gagal dibandingkan dengan pengamatan lama tak lengkap atau format/kunci berbeda. Bagian gabungan yang tak lengkap membuat kategori sebagian. Rincian cakupan menyimpan acuan dan area tetap. Tidak ada jaminan aman atau sebab-akibat.

## Acuan dan riwayat

Snapshot dapat dilihat, dinamai 1–120 karakter, dihapus, atau dijadikan acuan setelah konfirmasi. Ganti acuan sebelum menghapusnya. Pengguna, komputer, gabungan, level akses, dan pengamatan campuran lama memiliki acuan sendiri.

Tidak ada batas tetap checkpoint. Retensi opsional membersihkan snapshot lama tanpa nama dan melindungi checkpoint bernama serta semua acuan. Hasil sepenuhnya gagal tidak disimpan. Hapus riwayat menghapus snapshot/tanda setelah konfirmasi, mempertahankan preferensi/kunci, tidak menyentuh Windows atau ekspor. Bukan penghapusan forensik.

## Sumber dan batas

| Sumber | Batas |
| --- | --- |
| Aplikasi dan startup | Registrasi uninstall dan Run/RunOnce; bukan Store, portabel, atau folder startup. Registrasi bukan bukti eksekusi. |
| Layanan dan tugas | Konfigurasi terbaca; tanpa eksekusi, perintah/XML tersimpan, atau polling terus-menerus. |
| Pembaruan dan driver | Riwayat lokal sukses sampai 5.000 entri, lebih dari itu sebagian; metadata WMI. Tidak memasang, memeriksa firmware, atau rollback. |
| Default dan audio | Asosiasi didukung dan perangkat default; tidak merekam suara atau mengubahnya. |
| Perlindungan | Profil firewall, bukan penilaian antivirus. |
| Jaringan dan PATH | Aktif secara bawaan: proksi atau DNS/DHCP dan PATH tersimpan; tanpa paket, sandi, probe, variabel lain. |

Semua sumber yang didukung aktif secara bawaan jika belum ada pilihan valid tersimpan. Sumber yang dinonaktifkan tetap nonaktif setelah pembaruan; ubah di Sumber. Mengaktifkan tidak memulai pengumpulan atau menaikkan izin. Perbedaan membutuhkan pengamatan yang dapat digunakan di kedua sisi; snapshot lama tidak diisi ulang secara retroaktif.

25 detik per sumber. Inventaris layar menampilkan 1.000 item per sumber, semua data yang benar-benar ditangkap tetap disimpan. Hanya-baca masih menulis riwayat aplikasi dan ekspor yang Anda minta.

## Laporan

Laporan memakai hasil yang ditampilkan, bukan pilihan tanggal yang belum dijalankan. Pratinjau, salin, dan simpan teks di kedua mode; Lanjutan menambah JSON/CSV. Teks mengikuti bahasa, skema terstruktur tetap. Tak ada pengiriman otomatis.

Kunci, sidik jari, nilai peluncuran, nama checkpoint, dan ID audio tidak masuk laporan. Jalur profil dan pola rahasia disamarkan; rumus CSV dinetralkan. Nama pengenal bisa tersisa. PDF/HTML, impor, paket terenkripsi belum tersedia. Ekspor tidak terhapus bersama riwayat.

## Privasi dan penyimpanan

Lokasi biasa `%LOCALAPPDATA%\PCChangeTracker`; Pengaturan menunjukkan yang sebenarnya. SQLite tidak terenkripsi. Kunci memakai DPAPI pengguna saat ini; menyalinnya ke akun lain tidak menjamin dekripsi. Cadangkan dengan aman sebelum versi baru.

### Mengelola ruang disk

1. Lihat penyimpanan snapshot di atas Bantuan pada bilah kiri. Label terlihat di setiap halaman dan mencakup semua cakupan riwayat saat ini.
2. Arahkan penunjuk ke ukuran untuk panduan. Tab juga dapat memfokuskan label; pembaca layar menerima nama dan teks bantuannya.
3. Atur frekuensi pemeriksaan otomatis di Pengaturan. Interval lebih panjang menghasilkan lebih sedikit snapshot mendatang. Nonaktif menghentikan pengambilan otomatis, bukan menghapus riwayat atau menghentikan pembersihan retensi.
4. Retensi lebih singkat menghapus snapshot lama yang memenuhi syarat pada pembersihan berikutnya, bukan langsung saat dipilih. Pembersihan berjalan saat jatuh tempo, lalu setiap hari selama aplikasi berjalan dan setelah pengambilan otomatis berhasil. Acuan dan titik bernama dilindungi; ini bukan batas ruang yang mutlak.

Ukuran menjumlahkan `history.db`, `history.db-wal`, dan `history.db-shm` jika ada. Preferensi, ruang tambahan basis data dan ruang pakai ulang termasuk, bukan hanya data snapshot atau alokasi blok Windows. Ekspor, instalasi dan berkas kunci tidak termasuk. B, KiB, MiB, GiB dan TiB memakai kelipatan 1.024 dan format angka bahasa pilihan.

Nilai diperbarui bersama riwayat setelah pengambilan, penghapusan atau pembersihan, bukan terus-menerus. Ukuran tidak tersedia bukan berarti nol. Penghapusan dapat meninggalkan ruang pakai ulang tanpa mengecilkan berkas; riwayat kosong pun memakai ruang. Tidak ada pemadatan otomatis. Jangan hapus basis data atau berkas sementara saat aplikasi berjalan.

Format riwayat 2 menjaga rekaman lama sebagai campuran dan menolak pembaca lama. Hanya UI akses biasa yang menyimpan riwayat. Perilaku data MSIX perlu uji terpisah.

## Aksesibilitas

Pengaturan > Tampilan > Ukuran teks menawarkan 100%, 125%, 150%, dan 200%, menyimpan pilihan dan memperbesar halaman, kontrol, Bantuan serta Laporan. Pasangan bidang ditumpuk bila ruang sempit; halaman, bilah sisi, dan dialog dapat digulir. Pilihan font juga berlaku pada panduan; zoom dokumen Bantuan terpisah tetap sampai 160% dari ukuran dasar.

Navigasi samping mengumumkan halaman terpilih dan mendukung tombol panah. Ctrl+1 membuka perubahan, Ctrl+2 snapshot, Ctrl+3 sumber, Ctrl+4 pengaturan. F6 dan Shift+F6 berpindah antara navigasi, bilah perintah dan judul halaman; Tab melanjutkan ke kontrol. Di Bantuan, F6 berpindah antara pencarian, topik dan dokumen; Ctrl+F kembali ke pencarian.

Pemilih cakupan memfokuskan kotak pertama dan kembali ke Ubah cakupan setelah konfirmasi. Panel modal menonaktifkan seluruh bilah sisi dan pintasan halaman; Tab tetap di dalam. Escape menutup detail dan memulihkan fokus ke tindakan asal bila masih tersedia. Bantuan mulai pada pencarian, Laporan pada pratinjau hanya-baca. Baris snapshot, topik, kelompok dan bidang punya nama terbaca; nilai menyebut bidang dan sisi Sebelum/Sesudah, sumber menyebut status dan cakupan. Kontrol utama memiliki tinggi interaksi minimum 36 unit independen perangkat, di atas ukuran target minimum 24 piksel WCAG 2.2 AA; tombol yang dinonaktifkan menampilkan garis tepi putus-putus.

Ini bukan sertifikasi universal. Evaluasi manual dengan pembaca layar, tema kontras, skala Windows dan pengguna penyandang disabilitas tetap diperlukan. Pengujian papan ketik nyata membutuhkan sesi terbuka dan tidak terganggu.

Tab/Shift+Tab, panah, dan spasi menavigasi. Cakupan memakai kotak independen, mode memakai radio. Jenis/prioritas dijelaskan dengan teks selain warna. Fokus terlihat dan warna kontras tinggi Windows didukung.

F1 membuka bantuan, Ctrl+F mencari, Escape menutup. Zoom sampai 160%; tabel sempit menjadi entri berlabel. Uji lengkap pembaca layar dan penutur asli masih diperlukan.

Judul memiliki tingkat untuk navigasi pembaca layar. Membuka rincian memindahkan fokus ke panel; Tab tetap di dalamnya dan Escape menutupnya. Font dan warna yang mengikuti tema tersedia di Pengaturan; kontras tinggi diutamakan.

## Pemecahan masalah

Tanggal kosong: pilih rekaman lain. Perbandingan ditolak: cocokkan urutan, cakupan, akses. Sebagian bukan berarti dihapus. Laporan lama: jalankan pilihan baru dulu. Riwayat gagal dibuka: periksa ruang, izin, versi sebelum menghapus.

Sebagian pengaturan seluruh komputer memerlukan hak administrator; ChangeTracker melaporkannya tidak lengkap alih-alih meminta peningkatan izin, dan sumber lain tetap dibandingkan. Untuk dukungan kirim laporan yang sudah ditinjau dan versi app/Windows, jangan sandi, basis data mentah, atau kunci.

## Status rilis

Pratinjau dengan 11 kategori terbatas, pemeriksaan manual atau terjadwal opsional dan retensi yang dapat diatur. Belum ada pemantauan peristiwa terus-menerus, notifikasi atau linimasa lengkap. Pengumpulan memakai sumber daya, tidak menjanjikan nol CPU.

Rilis lokal memiliki installer MSI x64 dan ARM64 serta bundel MSIX x64, semuanya belum ditandatangani. Instalasi MSI memerlukan persetujuan administrator, tetapi aplikasi yang terpasang selalu berjalan dengan izin biasa. Penandatanganan, sertifikasi Store, kualifikasi Windows 10/ARM64, serta kualifikasi instal/upgrade/uninstall masih belum selesai. Perubahan kode tidak membangun ulang paket lama. Logo tidak menggantikan tangkapan layar asli atau sertifikasi.