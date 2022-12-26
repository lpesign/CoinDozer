mergeInto(LibraryManager.library, {
  GetFromLocalStorage: function (key, defaultValue) {
    var value = localStorage.getItem(UTF8ToString(key));

    if (!value && !defaultValue) {
      return null;
    } else if (!value && defaultValue) {
      value = UTF8ToString(defaultValue);
    }

    var bufferSize = lengthBytesUTF8(value) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(value, buffer, bufferSize);

    return buffer;
  },

  SetToLocalStorage: function (key, value) {
    localStorage[UTF8ToString(key)] = UTF8ToString(value);
  },

  DeleteFromLocalStorage: function (key) {
    localStorage.removeItem(UTF8ToString(key));
  },

  HasKeyInLocalStorage: function (key) {
    return !!localStorage.getItem(UTF8ToString(key));
  },
});
