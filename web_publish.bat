@ECHO OFF

docker build -f .\WeShareClone\Dockerfile -t registry.cajetan.dk/weshare-clone .

ECHO.

docker push registry.cajetan.dk/weshare-clone 

ECHO.
PAUSE