using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.U2D;

public class AddressableAssetManager : MonoBehaviour
{
    [SerializeField] protected List<SpriteAtlas> sprites = new();
    [SerializeField] protected List<SoundAsset> sounds = new();
    public Dictionary<string, float> progress = new();
    public AudioMixerGroup audioMixerGroup;


    public static AddressableAssetManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //Addressables.InitializeAsync();
    }

    public SoundAsset GetSoundAsset(string name)
    {
        foreach(var sound in sounds)
        {
            if(sound.name == name) 
                return sound;   
        }

        //{
        //    var obj = NewLoadSoundAsset(name).Result;
        //    sounds.Add(name, obj);
        //    return obj;
        //}

        return null;
    }

    public Sprite GetSprite(string name)
    {
        Sprite _sprite = null;
        foreach(var s in sprites)
        {
            _sprite = s.GetSprite(name);
            if (_sprite != null)
                return _sprite;
        }


        //_sprite = NewLoadSpriteAsset(name).Result;
        //return _sprite;

        return null;
        
    }

    /*
    public void LoadAll()
    {
        LoadSoundAssets();
        LoadSpritesAssets();
    }
    
    async public Task<object> LoadAsset(string name)
    {
        if (progress.ContainsKey(name))
            progress[name] = 0;
        else
            progress.Add(name, 0);

        var handle = Addressables.LoadAssetAsync<object>(name);
        while (handle.Task.IsCompleted)
        {
            progress[name] = handle.PercentComplete;
            await Task.Yield();
        }

        progress[name] = 1f;

        return handle.Task.Result;
    }

    async public Task<SoundAsset> NewLoadSoundAsset(string name)
    {
        if (progress.ContainsKey(name))
            progress[name] = 0;
        else
            progress.Add(name, 0);

            var handle = Addressables.LoadAssetAsync<SoundAsset>(name);
        while(handle.IsDone == false)
        {
            progress[name] = handle.PercentComplete;
            await Task.Yield();
        }

        progress[name] = 1f;

        return handle.Task.Result;
    }

    async public Task<Sprite> NewLoadSpriteAsset(string name)
    {
        if (progress.ContainsKey(name))
            progress[name] = 0;
        else
            progress.Add(name, 0);

        var handle = Addressables.LoadAssetAsync<Sprite>(name);
        while (handle.IsDone == false)
        {
            progress[name] = handle.PercentComplete;
            await Task.Yield();
        }

        progress[name] = 1f;

        return handle.Task.Result;
    }

    async public void LoadSoundAssets()
    {
        string lable = "SoundAsset";
        if (progress.ContainsKey(lable))
            progress[lable] = 0;
        else
            progress.Add(lable, 0);

        sounds.Clear();
        var handle = Addressables.LoadAssetsAsync<SoundAsset>(lable, callback: (asset) =>
        {
            sounds.Add(asset.name, asset);
        });

        while (handle.IsDone == false)
        {
            progress[lable] = handle.PercentComplete;
            await Task.Yield();
        }
        progress[lable] = 1f;

        Addressables.Release(handle);
    }

    async public void LoadSpritesAssets()
    {
        string lable = "Sprites";
        if (progress.ContainsKey(lable))
            progress[lable] = 0;
        else
            progress.Add(lable, 0);

        sprites.Clear();
        var handle = Addressables.LoadAssetsAsync<SpriteAtlas>(lable, callback: (asset) =>
        {
            sprites.Add(asset.name, asset);
        });

        while (handle.IsDone == false)
        {
            progress[lable] = handle.PercentComplete;
            await Task.Yield();
        }
        progress[lable] = 1f;

        Addressables.Release(handle);
    }
    */
}
