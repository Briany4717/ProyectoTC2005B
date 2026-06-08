using System;


[Serializable]
public class UserSong
{
    public string id_cancion;
    public string nombre_cancion;
    public string url_imagen;

    public string id => id_cancion;
    public string title => nombre_cancion;
    public string url => url_imagen;
}



