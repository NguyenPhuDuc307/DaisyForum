var accountController = function () {
    this.initialize = function () {
        loadLabels();
        registerEvents();
    };

    function loadLabels(pageIndex) {
        if (pageIndex === undefined) {
            pageIndex = 1;
        }
        var keyword = $('#txt_label').val();
        $.get(`/account/getLabels?keyword=${keyword}&page=${pageIndex}`)
            .done(function (response, statusText, xhr) {
                if (xhr.status === 200) {
                    const template = $('#tmpl_labels').html();

                    if (response?.items) {
                        let html = '';
                        response.items.forEach((item) => {
                            html += generateLabelHtml(item, template);
                        });
                        if (html) {
                            $('#label_list').html(html);
                        }
                        else {
                            $('#label_list').html('Không tìm thấy nhãn nào cho từ khóa \'' + keyword + '\'');
                        }
                    }
                }
            });
    }

    function generateLabelHtml(label, template) {
        return Mustache.render(template, {
            name: label.name
        });
    }

    function registerEvents() {

        $(document).ready(function () {
            $("#txt_label").on("keyup", function () {
                loadLabels();
            });
        });

        $('#btn_add_attachment').off('click').on('click', function () {
            $('#attachment_items').prepend('<p><input type="file" class="form-control" name="attachments" /></p>');
            return false;
        });

        $("#frm_new_kb").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.

            var form = $(this);
            form.validate();

            if (form.valid()) {
                var url = form.attr('action');
                var formData = false;
                if (window.FormData) {
                    formData = new FormData(form[0]);
                }

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function () {
                        window.location.href = '/bai-dang-cua-toi';
                    },
                    enctype: 'multipart/form-data',
                    processData: false,  // Important!
                    contentType: false,
                    beforeSend: function () {
                        $('#contact-loader').show();
                    },
                    cache: false,
                    error: function (err) {
                        $('#contact-loader').hide();
                        $('#message-result').html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result').append(errMsgs[field] + '<br>');
                            }
                        }
                    }
                });
            }
        });

        $("#frm_edit_kb").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.

            var form = $(this);
            form.validate();

            if (form.valid()) {
                var url = form.attr('action');
                var formData = false;
                if (window.FormData) {
                    formData = new FormData(form[0]);
                }

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (data) {
                        window.location.href = '/bai-dang-cua-toi';
                    },
                    enctype: 'multipart/form-data',
                    processData: false,  // Important!
                    contentType: false,
                    beforeSend: function () {
                        $('#contact-loader').show();
                    },
                    cache: false,
                    error: function (err) {
                        $('#contact-loader').hide();
                        $('#message-result').html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result').append(errMsgs[field] + '<br>');
                            }
                        }
                    }
                });
            }
        });
    }

};