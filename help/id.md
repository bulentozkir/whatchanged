# Bantuan ChangeTracker

Panduan luring untuk pratinjau. Aplikasi membaca konfigurasi, tidak memperbaiki Windows, mengubah pengaturan, atau menjamin PC aman.

## Bahasa

Pilih Bahasa di kiri. Pilihan disimpan dan memperbarui antarmuka, bantuan terbuka, tanggal, serta laporan teks tanpa mulai ulang atau pengumpulan. Dua puluh bahasa dibundel offline. Arab, Arab Mesir, dan Urdu memakai isi kanan-ke-kiri, menu tetap di kiri.

Nama aplikasi, label buatan pengguna, jalur, ID, dan nilai asli tidak diterjemahkan. JSON/CSV mempertahankan skema Inggris. Dialog Windows/UAC mengikuti bahasa Windows. Peninjauan penutur asli masih diperlukan sebelum rilis.

## Mulai

1. Buka biasa, bukan sebagai administrator.
2. Pengguna saat ini dan Seluruh komputer sama-sama dicentang untuk profil baru. Pilih satu atau keduanya, tidak boleh kosong, lalu konfirmasi.
3. Tinjau Sumber. Jaringan dan PATH opsional; mengubah pilihan tidak memulai pemeriksaan.
4. Pilih Hari ini dan Periksa sekarang.

Pembacaan pertama yang berguna menjadi acuan untuk cakupan dan akses itu. Ini inventaris, bukan rekonstruksi masa lalu. Tidak ada pengumpulan saat peluncuran atau pemantauan terus-menerus.

## Sederhana dan Lanjutan

Sederhana memberi ringkasan dan teks. Lanjutan menampilkan semua bidang, nilai sebelum/sesudah lengkap tanpa batas ringkasan, metadata, serta JSON/CSV. Kedua mode memiliki perbandingan tanggal, riwayat, dan sumber.

Pergantian mode tidak mengumpulkan, meningkatkan izin, memindahkan acuan, atau menagih biaya tambahan. Harga rencana US$0.99 sekali bayar mencakup keduanya. Pratinjau belum memiliki pembelian.

## Cakupan independen

Pengguna membaca registrasi aplikasi sendiri, Run/RunOnce, asosiasi, audio, proksi, PATH. Komputer membaca registrasi bersama, layanan, tugas, pembaruan, driver, firewall, DNS/DHCP, PATH sistem. Profil pribadi orang lain tidak dimuat.

Jika keduanya dipilih, bagian dibaca terpisah lalu menjadi satu snapshot gabungan. Hanya bagian komputer dapat memakai administrator melalui permintaan jelas. Bagian pengguna tetap memakai akun biasa semula. Pilihan tersimpan dihormati.

## Tanggal dan pengamatan

Tanggal menyaring hari lokal. Pilih juga waktu/checkpoint tepat karena ada beberapa snapshot sehari. Acuan memilih acuan akses biasa tanpa menggantinya. Hanya keadaan sekarang menghapus referensi sebelumnya.

Hari tanpa snapshot tidak dapat direkonstruksi dan tidak otomatis diganti hari terdekat. Kedua pengamatan harus berbeda, berurutan, tidak tumpang tindih, serta memiliki cakupan dan akses sama.

## Dua snapshot tersimpan

Pilih tanggal dan snapshot sebelumnya, lalu Snapshot tersimpan beserta tanggal dan snapshot berikutnya. Bandingkan snapshot tidak menjalankan pengumpul, UAC, atau membuat rekaman baru. Acuan tidak berubah. Waktu berbeda pada hari yang sama dapat dibandingkan.

## Snapshot dengan hari ini

Pilih referensi dan Hari ini. Periksa sekarang membuat pengamatan baru dan membandingkan pilihan Anda, bukan acuan lain diam-diam. Panel menutup setelah berhasil dan dapat dibuka lagi.

Referensi administrator tidak meningkatkan izin otomatis. Pilih tindakan administrator secara terpisah atau hanya keadaan sekarang. Batal atau tutup saat pengumpulan meminta berhenti dan mempertahankan riwayat lama.

## Administrator

Banyak pengaturan komputer dapat dibaca biasa. Yang dilindungi menjadi celah cakupan. Tindakan administrator memerlukan klik, konfirmasi bawaan Tidak, dan persetujuan Windows UAC untuk satu pemeriksaan.

Jendela utama tetap biasa. Pembantu sementara hanya-baca mengumpulkan bagian komputer, tidak memasang layanan atau menyimpan izin permanen. Penolakan tidak mengubah acuan/riwayat. Jangan bagikan sandi atau matikan kebijakan keamanan. Aplikasi tidak dapat menutup UAC yang sudah tampil; jawab melalui Windows.

## Rincian perubahan

Ditambahkan, Dihapus, Diubah menerangkan perbedaan dua pengamatan, bukan pelaku, waktu persis, atau sebab. Penting/Tinjau adalah prioritas, bukan putusan malware. Rutin/diharapkan dan dampak belum dinilai terpisah. Judul mengikuti filter; Lihat semua menampilkan lebih dari tiga awal.

Lanjutan memuat nilai panjang, konteks tetap, bidang baru/hilang, identitas dan sumber. Metadata mencakup ID, cakupan/akses, waktu UTC snapshot/pembacaan, versi, status, jumlah. Kosong berbeda dengan tidak ada. Perintah yang tak pernah disimpan tidak dapat dikembalikan; kunci dan sidik jari tetap tersembunyi. ID dapat mengenali perangkat: periksa sebelum menyalin.

Tanda diharapkan hanya untuk kejadian ini dan dapat dibatalkan. Buka pengaturan membuka alat Windows yang diizinkan tanpa memperbaiki apa pun.

## Cakupan dan ketidakpastian

Berhasil berarti lengkap dalam bagian yang diimplementasikan, bukan seluruh Windows. Sebagian berarti entri hilang atau batas; Gagal pembacaan tak berguna; Nonaktif/di luar cakupan berarti tidak dibaca. Data tak lengkap tidak berarti penghapusan.

Pengamatan saat ini yang lengkap dapat gagal dibandingkan dengan pengamatan lama tak lengkap atau format/kunci berbeda. Bagian gabungan yang tak lengkap membuat kategori sebagian. Rincian cakupan menyimpan acuan dan area tetap. Tidak ada jaminan aman atau sebab-akibat.

## Acuan dan riwayat

Snapshot dapat dilihat, dinamai 1–120 karakter, dihapus, atau dijadikan acuan setelah konfirmasi. Ganti acuan sebelum menghapusnya. Pengguna, komputer, gabungan, level akses, dan pengamatan campuran lama memiliki acuan sendiri.

Tidak ada pembersihan otomatis atau batas tetap checkpoint. Hasil sepenuhnya gagal tidak disimpan. Hapus riwayat menghapus snapshot/tanda setelah konfirmasi, mempertahankan preferensi/kunci, tidak menyentuh Windows atau ekspor. Bukan penghapusan forensik.

## Sumber dan batas

| Sumber | Batas |
| --- | --- |
| Aplikasi dan startup | Registrasi uninstall dan Run/RunOnce; bukan Store, portabel, atau folder startup. Registrasi bukan bukti eksekusi. |
| Layanan dan tugas | Konfigurasi terbaca; tanpa eksekusi, perintah/XML tersimpan, atau polling terus-menerus. |
| Pembaruan dan driver | Riwayat lokal sukses sampai 5.000 entri, lebih dari itu sebagian; metadata WMI. Tidak memasang, memeriksa firmware, atau rollback. |
| Default dan audio | Asosiasi didukung dan perangkat default; tidak merekam suara atau mengubahnya. |
| Perlindungan | Profil firewall, bukan penilaian antivirus. |
| Jaringan dan PATH | Opsional: proksi atau DNS/DHCP dan PATH tersimpan; tanpa paket, sandi, probe, variabel lain. |

25 detik per sumber. Inventaris layar menampilkan 1.000 item per sumber, semua data yang benar-benar ditangkap tetap disimpan. Hanya-baca masih menulis riwayat aplikasi dan ekspor yang Anda minta.

## Laporan

Laporan memakai hasil yang ditampilkan, bukan pilihan tanggal yang belum dijalankan. Pratinjau, salin, dan simpan teks di kedua mode; Lanjutan menambah JSON/CSV. Teks mengikuti bahasa, skema terstruktur tetap. Tak ada pengiriman otomatis.

Kunci, sidik jari, nilai peluncuran, nama checkpoint, dan ID audio tidak masuk laporan. Jalur profil dan pola rahasia disamarkan; rumus CSV dinetralkan. Nama pengenal bisa tersisa. PDF/HTML, impor, paket terenkripsi belum tersedia. Ekspor tidak terhapus bersama riwayat.

## Privasi dan penyimpanan

Lokasi biasa `%LOCALAPPDATA%\PCChangeTracker`; Pengaturan menunjukkan yang sebenarnya. SQLite tidak terenkripsi. Kunci memakai DPAPI pengguna saat ini; menyalinnya ke akun lain tidak menjamin dekripsi. Cadangkan dengan aman sebelum versi baru.

Format riwayat 2 menjaga rekaman lama sebagai campuran dan menolak pembaca lama. Pembantu hanya menerima kategori dan kunci sementara, bukan jalur riwayat atau perintah bebas. Hanya UI biasa menyimpan. Perilaku data MSIX perlu uji terpisah.

## Aksesibilitas

Tab/Shift+Tab, panah, dan spasi menavigasi. Cakupan memakai kotak independen, mode memakai radio. Jenis/prioritas dijelaskan dengan teks selain warna. Fokus terlihat dan warna kontras tinggi Windows didukung.

F1 membuka bantuan, Ctrl+F mencari, Escape menutup. Zoom sampai 160%; tabel sempit menjadi entri berlabel. Uji lengkap pembaca layar dan penutur asli masih diperlukan.

## Pemecahan masalah

Tanggal kosong: pilih rekaman lain. Perbandingan ditolak: cocokkan urutan, cakupan, akses. Sebagian bukan berarti dihapus. Laporan lama: jalankan pilihan baru dulu. Riwayat gagal dibuka: periksa ruang, izin, versi sebelum menghapus.

Untuk dukungan kirim laporan yang sudah ditinjau dan versi app/Windows, jangan sandi, basis data mentah, atau kunci. Menolak administrator tidak menghalangi pemeriksaan biasa.

## Status rilis

Pratinjau manual dengan 11 kategori terbatas; tanpa monitor kontinu, notifikasi, retensi terkonfigurasi atau linimasa lengkap. Pengumpulan memakai sumber daya, tidak menjanjikan nol CPU.

MSI/MSIX x64 lokal belum ditandatangani. Instalasi MSI memerlukan izin terpisah dari pengumpulan; jangan pasang kedua format bersamaan. UAC nyata, akun admin berbeda, Windows 10/ARM64, instalasi, dan persetujuan Store `allowElevation` belum dikualifikasi. Perubahan kode tidak membangun ulang paket lama. Logo tidak menggantikan tangkapan layar asli atau sertifikasi.