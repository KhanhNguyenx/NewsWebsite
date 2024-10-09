function GetContentsCKEditor(elm) {
    // Get the editor instance that you want to interact with.
    var editor = CKEDITOR.instances[elm];

    // Get editor content.
    // https://ckeditor.com/docs/ckeditor4/latest/api/CKEDITOR_editor.html#method-getData
    return editor.getData();
}

function InitCKEditorFull(elm) {
    CKEDITOR.replace(elm, {
        extraPlugins: 'image2,embed,emoji',
        height: 400,
        image2_disableResizer: true,
        //filebrowserImageUploadUrl: '/Common/ImageUploadCkEditor',
        uploadUrl: '/Common/ImageUploadCkEditor',
        //filebrowserBrowseUrl: '/Common/ImageUploadCkEditor',
        //filebrowserImageBrowseUrl: '/Common/ImageUploadCkEditor',

        "extraPlugins": 'imagebrowser',
        //"imageBrowser_listUrl": "/images/imageslist.json",
        "imageBrowser_listUrl": "/Common/ImageBrowser",

        filebrowserUploadUrl: '/Common/ImageUploadCkEditor',
        filebrowserImageUploadUrl: '/Common/ImageUploadCkEditor',
        removeDialogTabs: 'image:advanced;link:advanced',
        toolbar: [
            { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker'], items: ['Scayt'] },
            { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
            { name: 'insert', items: ['Image', 'Embed', 'Table', 'HorizontalRule', 'SpecialChar', 'EmojiPanel'] },
            { name: 'tools', items: ['Maximize'] },
            { name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source'] },
            { name: 'others', items: ['-'] },
            '/',
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Strike', '-', 'RemoveFormat'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock',] },
            { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
            { name: 'colors', items: ['TextColor', 'BGColor'] },
        ],
        language: 'vi',
        removeButtons: 'Underline,Strike,Subscript,Superscript,Anchor,Styles,Specialchar,PasteFromWord',
    });
}