#ps -ef|grep "pnpm"|grep -v grep|awk '{print $2}' |xargs kill
ps -ef|grep "/usr/bin/node.*dev$"|grep -v grep|awk '{print $2}' |xargs kill
