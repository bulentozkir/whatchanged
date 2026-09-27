# Trợ giúp ChangeTracker

Hướng dẫn ngoại tuyến cho bản phát triển. Ứng dụng quan sát cấu hình, không sửa Windows hay kết luận máy an toàn.

## Ngôn ngữ

Chọn **Cài đặt > Ngôn ngữ**. Lựa chọn được nhớ và cập nhật giao diện, trợ giúp đang mở, ngày và báo cáo văn bản mà không khởi động lại hoặc thu thập. Hai mươi ngôn ngữ có sẵn ngoại tuyến. Tiếng Ả Rập, Ả Rập Ai Cập và Urdu đọc phải sang trái, menu vẫn bên trái.

Tên ứng dụng, nhãn do bạn nhập, đường dẫn, ID và giá trị gốc không được dịch. JSON/CSV giữ trường tiếng Anh ổn định. Windows/UAC dùng ngôn ngữ hệ thống. Cần người bản ngữ kiểm duyệt trước phát hành.

## Cài đặt

Mở Cài đặt trong menu. Tùy chọn được lưu cho thư mục lịch sử hiện tại và khôi phục khi mở lại.

- Giao diện có chủ đề sáng/tối, phông chữ và màu riêng cho chữ ứng dụng, nhãn, nền nút và chữ nút. Mẫu màu có tên: Mặc định, Xanh hải quân, Xanh lá rừng, Đỏ sẫm và Tím. Mặc định khôi phục màu chủ đề; tương phản cao của Windows được ưu tiên và nút hành động chính giữ chữ tương phản. Nếu chưa lưu lựa chọn nào, chủ đề tối là mặc định. Các nút theo thứ bậc rõ ràng: kiểm tra chính màu xanh lam, thay mốc chuẩn màu hổ phách, thao tác xóa màu đỏ, còn mọi lệnh khác màu trung tính kèm biểu tượng có màu (ví dụ Báo cáo, Đổi phạm vi và Trợ giúp). Bảng màu được thiết kế cho người thị lực kém và mù màu: phần tương tác màu xanh lam, nhãn có màu riêng và mỗi trạng thái đều có thêm chữ và ký hiệu. Danh sách thả xuống, hộp kiểm và công tắc dùng màu nhấn cho mũi tên, dấu chọn và viền tiêu điểm. Các phần Cài đặt hiển thị theo cột nên trang thường vừa một màn hình mà không cần cuộn.
- Chụp tự động mặc định mỗi 4 giờ; lựa chọn đã lưu, kể cả Tắt, được giữ nguyên. Chọn mỗi 15 phút, 1 giờ, 4 giờ, 6 giờ, ngày hoặc tuần; chọn Tắt để chỉ kiểm tra thủ công. Chỉ chạy khi ứng dụng đang mở, kể cả trong khay hệ thống, dùng quyền thường và có thể hủy. Sau khi xác nhận phạm vi, lần đầu hoặc lần quá hạn có thể chạy ở lượt kiểm tra phút tiếp theo; các lần sau theo khoảng đã chọn. Không xin quản trị, đánh thức máy hay chạy lại mọi khoảng đã bỏ lỡ.
- Mặc định giữ dữ liệu 30 ngày; lựa chọn đã lưu, kể cả giữ mãi mãi, được giữ nguyên. Chọn 30, 90, 180 hoặc 365 ngày, hoặc mãi mãi. Chỉ xóa ảnh cũ không tên và không phải mốc chuẩn. Dọn dẹp chạy khi đến hạn lần đầu, sau đó hằng ngày khi ứng dụng mở và sau kiểm tra tự động thành công, kể cả khi chụp tự động tắt. Điểm có tên và mọi mốc chuẩn đều được bảo vệ.
- Khởi động khi đăng nhập là tùy chọn, mặc định tắt. Bao gồm đăng nhập sau khi khởi động lại, không thu thập trước đăng nhập. Chỉ sửa mục khởi động riêng của ứng dụng cho người dùng này; không cài dịch vụ hoặc tác vụ lúc máy khởi động, không sửa ứng dụng khác hay chính sách. Nếu thất bại, giữ lựa chọn cũ.
- Thu nhỏ hoặc Đóng luôn ẩn cửa sổ vào khay và tiếp tục kiểm tra. Mở, nhấp đúp biểu tượng hoặc chạy ứng dụng lần nữa khôi phục cửa sổ. Thoát từ khay hủy công việc đang chạy và kết thúc. Khởi động thông thường mở cửa sổ phóng to; khôi phục từ khay giữ trạng thái hiển thị trước đó. Khởi động khi đăng nhập là tùy chọn và ẩn cửa sổ, kể cả sau khi khởi động lại Windows; không chạy trước đăng nhập hay như dịch vụ.

Đơn giản hiển thị trường công khai đã đổi với nhãn Trước/Sau và giá trị lớn hơn, có thể chọn nhưng chỉ đọc. Đóng chi tiết trả tiêu điểm về nút ban đầu nếu còn khả dụng. Nâng cao tách ngày còn lưu khỏi giờ chụp chính xác, với điểm kiểm tra và phạm vi/quyền trên các dòng riêng. Đổi chế độ giữ nguyên cặp so sánh.

Hồ sơ mới chọn cả hai phạm vi; lựa chọn đã lưu được giữ. Có thể xem mọi lịch sử còn lưu dù phạm vi hiện tại khác, nhưng hai đầu so sánh phải cùng phạm vi và quyền. Tùy chọn không cấp quyền quản trị. Đo tài nguyên và kiểm định gói đã cài vẫn chưa hoàn tất.

## Bắt đầu

1. Mở bình thường, không chạy quản trị.
2. Hồ sơ mới chọn cả Người dùng hiện tại và Toàn máy tính. Giữ một hoặc cả hai, không được bỏ cả hai, rồi xác nhận.
3. Xem Nguồn. Mọi kiểm tra được hỗ trợ, gồm Mạng và PATH, mặc định bật. Lựa chọn đã lưu được giữ nguyên; bạn có thể tắt nguồn. Chọn không bắt đầu kiểm tra.
4. Chọn Hôm nay và Kiểm tra ngay.

Quan sát hữu ích đầu tiên làm mốc cho phạm vi/quyền đó. Đây là danh sách hiện tại, không tái tạo quá khứ. Hồ sơ mới dùng khoảng 4 giờ, chỉ sau khi xác nhận phạm vi và khi ứng dụng đang chạy. Chọn Tắt để chỉ kiểm tra thủ công.

## Đơn giản và Nâng cao

Đơn giản có tóm tắt và văn bản. Nâng cao thêm mọi trường đã thu thập, giá trị trước/sau không giới hạn tóm tắt, siêu dữ liệu và JSON/CSV. Ngày, lịch sử, nguồn đều có ở cả hai.

Đổi chế độ không thu thập, nâng quyền, chuyển mốc hay tính phí thêm. Giá dự kiến 0,99 USD một lần gồm cả hai chế độ. Bản xem trước chưa có mua hàng.

## Phạm vi độc lập

Người dùng đọc đăng ký app, Run/RunOnce, liên kết mặc định, âm thanh, proxy, PATH của mình. Máy đọc đăng ký chung, dịch vụ, tác vụ, cập nhật, driver, firewall, DNS/DHCP, PATH hệ thống. Không tải hồ sơ riêng tư người khác.

Chọn cả hai sẽ đọc riêng bằng quyền thường trong chính tài khoản của bạn rồi gộp thành một ảnh. Không phạm vi nào được cấp quyền quản trị. Tôn trọng lựa chọn và nguồn đã lưu.

## Ngày và quan sát

Bộ chọn chỉ liệt kê ảnh còn lưu với ngày địa phương, giờ gồm mili giây, độ lệch UTC, điểm và phạm vi/quyền. Không nhập ngày tùy ý; ảnh đã xóa biến mất khỏi danh sách. Đầu thứ hai có thể là ảnh đã lưu hoặc một kiểm tra Hôm nay mới. Mốc chuẩn chỉ chọn mốc quyền thường, không thay nó. Chỉ trạng thái hiện tại xóa lựa chọn trước. Tiêu đề **So sánh** luôn nêu cả hai đầu của lựa chọn nên vẫn đọc được khi thu gọn; tiêu đề thu gọn sau khi kiểm tra hoặc so sánh và khi bạn mở trang khác.

Ngày chưa có ảnh không thể tái tạo, không tự chọn ngày gần nhất. Hai quan sát phải khác nhau, theo thời gian, không chồng lấn và cùng phạm vi/quyền.

## Hai ảnh đã lưu

Chọn ngày/ảnh trước, rồi Ảnh chụp đã lưu và ngày/ảnh sau. So sánh không chạy bộ thu thập, không UAC, không tạo bản ghi. Mốc giữ nguyên. Hai giờ khác nhau cùng ngày vẫn so sánh được.

## Ảnh với hôm nay

Chọn ảnh cũ và Hôm nay. Kiểm tra ngay tạo quan sát mới và so với đúng lựa chọn, không bí mật đổi mốc. Bảng chọn thu gọn sau thành công và có thể mở lại.

Ảnh do phiên bản cũ lưu bằng quyền quản trị không thể làm mốc cho kiểm tra mới vì mọi kiểm tra luôn dùng quyền thường. Chọn ảnh quyền thường hoặc Chỉ trạng thái hiện tại. Hai ảnh đã lưu vẫn so sánh được với nhau. Hủy dừng thu thập và giữ lịch sử. Đóng vẫn tiếp tục thu thập trong khay; Thoát từ khay hủy và kết thúc ứng dụng.

## Quyền quản trị

ChangeTracker không bao giờ xin quyền quản trị. Mọi kiểm tra, thủ công hay tự động, đều chạy bằng quyền Windows thường trong cả hai phạm vi, nên Windows không hiện lời nhắc UAC cho một kiểm tra. Không có chế độ quản trị, trình phụ nâng quyền hay dịch vụ nền.

Nhiều thiết lập toàn máy đọc được bằng quyền thường. Khi một nguồn có nội dung quyền thường không đọc được, nguồn đó được báo không đầy đủ (liệt kê trong chi tiết phạm vi, không bao giờ gợi ý xóa) thay vì nâng quyền. Không hỗ trợ khởi động ChangeTracker bằng “Chạy với tư cách quản trị viên”: ứng dụng hiện thông báo rồi đóng; hãy mở bình thường.

Ảnh do phiên bản cũ lưu bằng quyền quản trị vẫn nằm trong lịch sử: có thể xem, so sánh với nhau và đưa vào báo cáo, nhưng không thể làm ảnh trước cho kiểm tra mới; hãy chọn ảnh quyền thường hoặc “Chỉ trạng thái hiện tại”. Cài MSI cần Windows phê duyệt quản trị (chỉ cài đặt, không phải kiểm tra); gói Microsoft Store cài không cần điều đó. Đừng bao giờ chia sẻ mật khẩu quản trị.

## Chi tiết thay đổi

Thêm, Xóa, Sửa mô tả hai quan sát, không cho biết ai, thời điểm chính xác hay nguyên nhân. Quan trọng/Cần xem là ưu tiên, không phán quyết mã độc. Hoạt động thông thường/dự kiến và tác động chưa đánh giá tách riêng. Tiêu đề theo bộ lọc; Xem tất cả cho các mục sau ba mục đầu.

Nâng cao hiển thị giá trị dài, ngữ cảnh không đổi, trường mới/mất, ID và nguồn. Siêu dữ liệu gồm ID ảnh, phạm vi/quyền, thời gian UTC thu/đọc, trạng thái, phiên bản, số lượng. Rỗng khác với không tồn tại. Lệnh chưa từng lưu không thể phục hồi; khóa và dấu vân tay ẩn. ID có thể nhận diện máy, nên kiểm tra trước sao chép.

Dấu dự kiến chỉ cho lần này và hoàn tác được. Mở cài đặt chỉ mở công cụ Windows được phép, không sửa chữa.

## Phạm vi và bất định

Thành công là đủ trong phần đã triển khai, không phải toàn Windows. Một phần là thiếu mục/giới hạn; Thất bại là đọc vô ích; Tắt/ngoài phạm vi là chưa đọc. Không suy diễn xóa từ dữ liệu thiếu.

Ảnh mới đủ vẫn có thể không so được với ảnh cũ thiếu hoặc khác định dạng/khóa. Một phần gộp thiếu làm cả danh mục thành một phần. Chi tiết phạm vi giữ mốc và vùng không đổi. Không khác biệt không bảo đảm an toàn hay quan hệ nhân quả.

## Mốc và lịch sử

Trang ảnh cho xem, đặt tên 1–120 ký tự, xóa, thay mốc sau xác nhận. Thay mốc trước khi xóa mốc hiện tại. Người dùng, máy, cả hai, quyền khác nhau và ảnh hỗn hợp cũ có mốc riêng.

Không có giới hạn điểm cố định. Thời hạn lưu tùy chọn dọn ảnh cũ không tên và bảo vệ điểm có tên cùng mọi mốc chuẩn. Kết quả hoàn toàn vô ích không lưu. Xóa lịch sử có xác nhận, xóa ảnh/dấu nhưng giữ tùy chọn/khóa; không ảnh hưởng Windows hoặc tệp xuất, không phải xóa pháp chứng.

## Nguồn và giới hạn

| Nguồn | Giới hạn |
| --- | --- |
| App và khởi động | Đăng ký gỡ cài và Run/RunOnce; không Store, app di động, thư mục Startup. Đăng ký không chứng minh chạy. |
| Dịch vụ và tác vụ | Cấu hình đọc được; không thực thi, lưu lệnh/XML hay thăm dò liên tục. |
| Cập nhật và driver | Lịch sử thành công cục bộ tối đa 5.000 sự kiện, vượt là một phần; WMI. Không cài, dò firmware hay hoàn nguyên. |
| Mặc định và âm thanh | Liên kết được hỗ trợ, thiết bị mặc định; không ghi âm hay chỉnh sửa. |
| Bảo vệ | Hồ sơ firewall, không đánh giá chống virus. |
| Mạng và PATH | Mặc định bật: proxy hoặc DNS/DHCP và PATH lưu; không gói tin, mật khẩu, dò mạng hay biến khác. |

Mọi nguồn được hỗ trợ mặc định bật nếu chưa có lựa chọn hợp lệ đã lưu. Nguồn đã tắt vẫn tắt sau cập nhật; đổi trong Nguồn. Bật không bắt đầu thu thập hoặc nâng quyền. Cần quan sát hợp lệ ở cả hai đầu để hiển thị khác biệt; ảnh cũ không được bổ sung dữ liệu hồi tố.

Mỗi nguồn 25 giây. Danh sách hiển thị tối đa 1.000 mục/nguồn, giữ toàn bộ dữ liệu thực sự thu được. Chỉ đọc vẫn ghi lịch sử riêng và tệp xuất do bạn yêu cầu.

## Báo cáo

Báo cáo dùng kết quả đang hiển thị, không lựa chọn chưa chạy. Cả hai chế độ có xem/copy/lưu văn bản; Nâng cao thêm JSON/CSV. Văn bản theo ngôn ngữ, lược đồ cấu trúc không đổi. Không tự gửi.

Khóa, dấu vân tay, giá trị lệnh, tên điểm và ID âm thanh được bỏ khỏi báo cáo. Đường dẫn hồ sơ và mẫu bí mật được che, công thức CSV vô hiệu hóa. Tên nhận diện có thể còn. Chưa có PDF/HTML, nhập và gói mã hóa. Xóa lịch sử không xóa tệp xuất.

## Riêng tư và lưu trữ

Thường ở `%LOCALAPPDATA%\PCChangeTracker`; Cài đặt hiển thị đường dẫn thật. SQLite không mã hóa. Khóa dùng DPAPI người dùng hiện tại; sao sang tài khoản khác không bảo đảm giải mã. Sao lưu an toàn trước bản mới.

### Quản lý dung lượng đĩa

1. Xem dung lượng ảnh chụp phía trên Trợ giúp ở thanh trái. Nhãn xuất hiện trên mọi trang và gồm mọi phạm vi của lịch sử hiện tại.
2. Di chuột lên dung lượng để xem hướng dẫn. Tab cũng đưa tiêu điểm đến nhãn; trình đọc màn hình nhận tên và nội dung trợ giúp.
3. Điều chỉnh tần suất kiểm tra tự động trong Cài đặt. Khoảng cách dài hơn tạo ít ảnh mới hơn. Tắt dừng chụp tự động, không xóa lịch sử và không dừng dọn dẹp theo thời hạn.
4. Thời hạn ngắn hơn xóa ảnh cũ đủ điều kiện ở lần dọn dẹp đến hạn tiếp theo, không ngay khi chọn. Dọn dẹp chạy khi đến hạn, rồi hằng ngày lúc ứng dụng mở và sau lần chụp tự động thành công. Ảnh cơ sở và điểm có tên được bảo vệ; đây không phải giới hạn dung lượng tuyệt đối.

Dung lượng cộng các tệp `history.db`, `history.db-wal` và `history.db-shm` khi có. Bao gồm tùy chọn, phần phụ trợ và chỗ trống dùng lại, không chỉ dữ liệu ảnh hay kích thước cấp phát theo khối của Windows. Không gồm bản xuất, cài đặt và tệp khóa. B, KiB, MiB, GiB, TiB dùng bội số 1.024 và định dạng số của ngôn ngữ đã chọn.

Giá trị cập nhật cùng lịch sử sau chụp, xóa hoặc dọn dẹp, không theo dõi liên tục. Không đọc được dung lượng không có nghĩa là không chiếm chỗ. Xóa có thể để lại chỗ dùng lại mà không làm tệp nhỏ hơn; lịch sử trống vẫn có phần phụ trợ. Không tự nén cơ sở dữ liệu. Đừng xóa cơ sở dữ liệu hoặc tệp tạm khi ứng dụng đang chạy.

Lịch sử phiên bản 2 giữ ảnh cũ là hỗn hợp và chặn trình đọc cũ. Chỉ giao diện quyền thường lưu lịch sử. Vòng đời MSIX cần kiểm tra riêng.

## Trợ năng

Cài đặt > Giao diện > Cỡ chữ có 100%, 125%, 150%, 200%, lưu lựa chọn và phóng chữ trên trang, điều khiển, Trợ giúp và Báo cáo. Cặp trường xếp dọc khi thiếu chỗ; trang, thanh bên và hộp thoại có thể cuộn. Phông chữ chọn cũng áp dụng cho tài liệu trợ giúp; mức thu phóng riêng vẫn đến 160% cỡ cơ sở.

Điều hướng bên thông báo trang đã chọn và dùng phím mũi tên. Ctrl+1 mở thay đổi, Ctrl+2 ảnh chụp, Ctrl+3 nguồn, Ctrl+4 cài đặt. F6 và Shift+F6 chuyển giữa điều hướng, thanh lệnh và tiêu đề trang; Tab tiếp tục vào điều khiển. Trong Trợ giúp, F6 chuyển giữa tìm kiếm, chủ đề và tài liệu; Ctrl+F về tìm kiếm.

Bộ chọn phạm vi đặt tiêu điểm vào ô đầu tiên, rồi về Đổi phạm vi khi xác nhận. Bảng phương thức vô hiệu hóa toàn bộ thanh bên và phím tắt trang; Tab ở bên trong. Escape đóng chi tiết và trả tiêu điểm về thao tác gốc nếu còn. Trợ giúp bắt đầu ở tìm kiếm, Báo cáo ở bản xem chỉ đọc. Hàng ảnh, chủ đề, nhóm và trường có tên dễ đọc; giá trị nói rõ trường và bên Trước/Sau, nguồn nói rõ trạng thái và phạm vi. Điều khiển chính có chiều cao tương tác ít nhất 36 đơn vị độc lập thiết bị, cao hơn kích thước mục tiêu tối thiểu 24 pixel của WCAG 2.2 AA; nút bị tắt hiển thị viền nét đứt.

Đây không phải chứng nhận toàn diện. Vẫn cần đánh giá thủ công với trình đọc màn hình, chủ đề tương phản, tỉ lệ Windows và người khuyết tật. Thử nghiệm bàn phím thực cần phiên đã mở khóa và không bị can thiệp.

Tab/Shift+Tab, mũi tên, Space để thao tác. Phạm vi là ô đánh dấu độc lập, chế độ là nút radio. Loại/ưu tiên có chữ, không chỉ màu. Có tiêu điểm rõ và màu tương phản cao Windows.

F1 mở trợ giúp, Ctrl+F tìm, Escape đóng. Phóng chữ tới 160%; bảng hẹp thành mục có nhãn. Kiểm định đầy đủ trình đọc màn hình và người bản ngữ còn chờ.

Tiêu đề có cấp độ cho trình đọc màn hình. Mở chi tiết đưa tiêu điểm vào bảng; Tab chỉ đi trong bảng, Escape đóng. Cài đặt cho chọn phông chữ và màu theo chủ đề; tương phản cao được ưu tiên.

## Khắc phục sự cố

Ngày trống: chọn quan sát khác. Từ chối so sánh: kiểm tra thứ tự, phạm vi/quyền. Một phần không có nghĩa bị xóa. Báo cáo cũ: chạy lựa chọn mới. Không mở cơ sở dữ liệu: kiểm tra dung lượng, quyền, phiên bản trước xóa.

Một số thiết lập toàn máy cần quyền quản trị; ChangeTracker báo chúng không đầy đủ thay vì xin nâng quyền, còn các nguồn khác vẫn được so sánh. Gửi hỗ trợ báo cáo đã xem cùng phiên bản app/Windows, không mật khẩu, cơ sở dữ liệu thô hay khóa.

## Phát hành

Bản xem trước có 11 nhóm giới hạn, kiểm tra thủ công hoặc theo lịch tùy chọn và thời hạn lưu tùy chỉnh. Chưa có giám sát sự kiện liên tục, thông báo hay dòng thời gian đầy đủ. Thu thập tốn tài nguyên, không hứa CPU bằng không.

Bản phát hành cục bộ có bộ cài MSI x64 và ARM64 cùng gói MSIX x64, tất cả chưa ký. Cài MSI cần phê duyệt quản trị, nhưng ứng dụng đã cài luôn chạy bằng quyền thường. Ký số, chứng nhận Store, kiểm định Windows 10/ARM64 và kiểm định cài đặt/nâng cấp/gỡ cài đặt vẫn còn thiếu. Đổi nguồn không xây lại gói cũ. Logo không thay ảnh chụp thực tế hay chứng nhận.