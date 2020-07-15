#!/usr/bin/env bash

echo archiving ...
cd ../build || exit 1

tar -czf ../build.tgz *

cd .. || exit 1


echo uploading...
scp -i ~/.key/hashtag_2_key build.tgz tdr@tdrs.ro:/var/www/tdrs.ro/infoeducatie/portal
echo uploaded

echo running deploy script
ssh -i ~/.key/hashtag_2_key tdr@tdrs.ro "/var/www/tdrs.ro/infoeducatie/portal/deploy.sh"
echo done
