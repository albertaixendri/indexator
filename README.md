# Indexator

## About

This class library allows to make fuzzy searches using keywords over a set of documents previously indexed. It means that keywords can contain mistakes like typos or orthografic errors. Additionally it can give a list of common keyword subsets found among the documents.

This project is developed with Dot Net Core 2.0 and functional programming is used to code.

## Getting started

1. Download or clone this project.
1. Download and unzip any corpora from <http://wortschatz.uni-leipzig.de/en/download>.
1. On a terminal type go to the downloaded/cloned project and type:

```bash
dotnet run --project InteractiveShell/InteractiveShell.csproj
```

Now you can load and index the corpora:

``` bash
> l /Users/albert/Downloads/eng_news_2015_10K/eng_news_2015_10K-sentences.txt
.....
19/1/18 6:49:49 9985 WF:24334 R:777150 KSTD: 24334E 1598651 CE 16 DE 1594427
19/1/18 6:49:49 9986 WF:24338 R:777337 KSTD: 24338E 1597736 CE 16 DE 1593512
19/1/18 6:49:49 9987 WF:24342 R:777423 KSTD: 24342E 1596730 CE 16 DE 1592503
```

Once the file is indexed you can do searches or index common keyword subsets

``` text
> t
23 that, more, than

21 Content, preferences, Done

15 have, that, been

13 This, story, been, viewed, times
This, story, been, viewed
This, story, been, times
This, story, viewed, times
This, been, viewed, times
story, been, viewed, times
This, story, been
This, story, viewed
This, story, times
This, been, viewed
.....
```

``` text
> s Apple
Found keys: Apple(Apple)
Showing first 25 results
00000000-0000-0000-0000-000000000664
00000000-0000-0000-0000-000000000665
00000000-0000-0000-0000-000000000666
00000000-0000-0000-0000-000000000667
00000000-0000-0000-0000-000000000668
00000000-0000-0000-0000-000000000669
....

> s Apple made
Found keys: Apple(Apple) made(made)
Showing first 25 results
00000000-0000-0000-0000-000000000666
00000000-0000-0000-0000-000000001463

> s Apple made
Found keys: Apple(Apple) made(made)
Showing first 25 results
00000000-0000-0000-0000-000000000666
00000000-0000-0000-0000-000000001463

> s Aple madde
Found keys: Aple(Apple) madde(made)
Showing first 25 results
00000000-0000-0000-0000-000000000666
00000000-0000-0000-0000-000000001463

```

## Future work

A lot of work to be done:

* Documentation!!
* More unit tests to give more stability.
* Extract hardcoded parameters as configuration variables.
* Use of remote dictionaries like Redis.
* Use of multiple threads to do work.
* Client / server arquitecture.

Other possible idea: ports to other languages/platforms that are pure functional programming like F#, Erlang...