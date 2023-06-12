var plugin = {
    copyToClipboard: function(test){
        navigator.clipboard.writeText(UTF8ToString(test))
    },
    gotoBhaariWebsite: function(){
        window.location.href="https://bhaari.com/"
    },
    getRecipeData:function(text,mapid){
        //console.log("Data sent"+UTF8ToString(text))
        var xhr = new XMLHttpRequest();
        var url = "https://square.bhaari.com/api/recipes/create-recipe";
        xhr.open("POST", url, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                //var json = JSON.parse(xhr.responseText);
                //console.log(json);
                unitySender.SendMessage("RecipeDiv","PutRecipeData",xhr.responseText)
            }
        };
        var data = JSON.stringify({
            "data": "Write a recipe that only uses following ingredients:"+UTF8ToString(text)+". First 24 characters should be dish name.",
            "nameData": "-",
            "mapid": UTF8ToString(mapid)
        });
        xhr.send(data);
    }
};

mergeInto(LibraryManager.library, plugin);